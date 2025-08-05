using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace PuckAIPractice.AI
{
    public class GoalieAI : MonoBehaviour
    {
        Vector3 redGoal = new Vector3(0.0f, 0.8f, -40.23f);
        Vector3 blueGoal = new Vector3(0.0f, 0.8f, 40.23f);
        public Player controlledPlayer;
        public Transform puckTransform;
        public PlayerTeam team;
        private GameObject puckLine;
        private bool lineInitialized = false;
        private float lastDashStartTime = -Mathf.Infinity;
        private float lastDashTime = -Mathf.Infinity;
        private bool dashLeftNext = true;
        bool hasDashed = false;
        private PlayerBodyV2 body;
        private Vector3? targetCancelPosition = null;
        private float cancelInterpSpeed = 20f; // tweak this value to control smoothness

        void Start()
        {
            Debug.Log("Goalie AI Started");

            if (controlledPlayer != null)
                body = controlledPlayer.PlayerBody;

            //StartCoroutine(DelayedSlide());
        }
        IEnumerator DelayedSlide()
        {
            yield return new WaitForSeconds(3f);
            var body = controlledPlayer.PlayerBody;
            if (body != null)
            {
                Debug.Log("Found Body Updating Slide");
                body.IsSliding.Value = true;
            }

        }
        private bool isDashing = false;
        private GameObject interceptTargetSphere;
        void FixedUpdate()
        {
            if (targetCancelPosition.HasValue)
            {
                Debug.Log("Cancelling Dash!");

                Vector3 current = body.transform.position;
                Vector3 target = targetCancelPosition.Value;
                Debug.Log(targetCancelPosition.Value);
                body.Rigidbody.MovePosition(targetCancelPosition.Value);
                targetCancelPosition = null;
                hasDashed = false;
                //// Smooth step toward cancel point
                //Vector3 next = Vector3.MoveTowards(current, target, cancelInterpSpeed * Time.fixedDeltaTime);

                //body.Rigidbody.MovePosition(next);

                //// Close enough? Snap and stop
                //if (Vector3.Distance(next, target) < 0.01f)
                //{
                //    body.Rigidbody.MovePosition(target);
                //    hasDashed = false;
                //    targetCancelPosition = null;
                //}
            }
        }
        void Update()
        {
            if (controlledPlayer == null) return;

            ResolvePuckReference();
            InitializeBody();
            InitializeInterceptVisuals();

            Quaternion neutralRotation = GetNeutralRotation();
            Vector3 neutralForward = GetNeutralForward();

            Vector3 goaliePos = controlledPlayer.PlayerBody.Rigidbody.transform.position;
            Vector3 puckPos = puckTransform?.position ?? Vector3.zero;

            Vector3 toPuck = puckPos - goaliePos;
            toPuck.y = 0f;

            RotateTowardPuck(toPuck, neutralForward);

            Vector3 goalCenter = (controlledPlayer.Team.Value == PlayerTeam.Red) ? redGoal : blueGoal;
            Vector3 projectedPoint = GetProjectedIntercept(goaliePos, puckPos, goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir);
            UpdateInterceptVisual(projectedPoint);

            Vector3 goalieToPuck = puckPos - goaliePos;
            Vector3 goalieForward = neutralForward;
            float forwardDot = Vector3.Dot(goalieForward, goalieToPuck.normalized);
            if (forwardDot < 0.5f) return;

            HandleDashLogic(toPuck, neutralRotation, lateralDistance, signedLateralOffset, projectedPoint);

            UpdatePuckLine(goalCenter, puckPos);
        }
        private void ResolvePuckReference()
        {
            if (puckTransform != null) return;
            var puck = NetworkBehaviourSingleton<PuckManager>.Instance.GetPlayerPuck(NetworkBehaviourSingleton<PuckManager>.Instance.OwnerClientId);
            if (puck != null) puckTransform = puck.transform;
        }

        private void InitializeBody()
        {
            if (body != null) return;
            body = controlledPlayer.GetComponent<PlayerBodyV2>();
        }

        private void InitializeInterceptVisuals()
        {
            if (interceptTargetSphere != null) return;

            interceptTargetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            interceptTargetSphere.name = "InterceptTargetSphere";
            interceptTargetSphere.GetComponent<Collider>().enabled = false;

            var mat = new Material(Shader.Find("Legacy Shaders/Diffuse"));
            mat.color = new Color(1f, 0.5f, 0f, 0.7f);
            interceptTargetSphere.GetComponent<Renderer>().material = mat;
            interceptTargetSphere.transform.localScale = Vector3.one * 0.3f;
        }

        private Quaternion GetNeutralRotation()
        {
            return Quaternion.LookRotation(
                controlledPlayer.Team.Value == PlayerTeam.Red ? Vector3.forward : Vector3.back,
                Vector3.up);
        }

        private Vector3 GetNeutralForward()
        {
            return controlledPlayer.Team.Value == PlayerTeam.Red ? Vector3.forward : Vector3.back;
        }

        private void RotateTowardPuck(Vector3 toPuck, Vector3 neutralForward)
        {
            if (toPuck.sqrMagnitude <= 0.01f) return;

            Vector3 desiredDir = toPuck.normalized;
            float angleBetween = Vector3.SignedAngle(neutralForward, desiredDir, Vector3.up);
            float clampedAngle = Mathf.Clamp(angleBetween, -GoalieSettings.Instance.MaxRotationAngle, GoalieSettings.Instance.MaxRotationAngle);
            Quaternion targetRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up) * Quaternion.LookRotation(neutralForward);

            body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation,
                targetRotation,
                Time.deltaTime * GoalieSettings.Instance.RotationSpeed
            );
        }

        private Vector3 GetProjectedIntercept(Vector3 goaliePos, Vector3 puckPos, Vector3 goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir)
        {
            Vector3 puckToGoal = goalCenter - puckPos;
            puckToGoalDir = puckToGoal.normalized;

            Vector3 puckToGoalie = goaliePos - puckPos;
            float projectedLength = Vector3.Dot(puckToGoalie, puckToGoalDir);
            Vector3 projectedPoint = puckPos + puckToGoalDir * projectedLength;
            projectedPoint.y = body.transform.position.y;

            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = projectedPoint;

            Vector3 correctionVector = projectedPoint - goaliePos;
            Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);

            lateralDistance = correctionVector.magnitude;
            signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);

            return projectedPoint;
        }

        private void HandleDashLogic(Vector3 toPuck, Quaternion neutralRotation, float lateralDistance, float signedLateralOffset, Vector3 projectedPoint)
        {
            bool cooldownActive = Time.time < lastDashTime + GoalieSettings.Instance.DashCooldown;
            bool gracePeriodActive = Time.time < lastDashStartTime + GoalieSettings.Instance.DashCancelGrace;

            if (lateralDistance > GoalieSettings.Instance.DashThreshold && !cooldownActive)
            {
                controlledPlayer.PlayerInput.Client_SlideInputRpc(true);
                body.transform.rotation = neutralRotation;

                if (signedLateralOffset < 0)
                    controlledPlayer.PlayerInput.Client_DashRightInputRpc();
                else
                    controlledPlayer.PlayerInput.Client_DashLeftInputRpc();

                hasDashed = true;
                lastDashTime = Time.time;
            }
            else if (hasDashed && Mathf.Abs(signedLateralOffset) <= GoalieSettings.Instance.CancelThreshold)
            {
                body.CancelDash();
                targetCancelPosition = projectedPoint;
                Debug.Log($"[GoalieAI] Perfect alignment, canceling dash. Offset = {signedLateralOffset:F2}");
            }
        }

        private void UpdatePuckLine(Vector3 start, Vector3 end)
        {
            if (!lineInitialized)
            {
                puckLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
                puckLine.name = "PuckInterceptLine";
                puckLine.GetComponent<Collider>().enabled = false;

                var renderer = puckLine.GetComponent<Renderer>();
                renderer.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
                renderer.material.color = new Color(0f, 1f, 1f, 0.5f);
                lineInitialized = true;
            }

            if (puckLine != null)
            {
                Vector3 midPoint = (start + end) / 2f;
                Vector3 direction = end - start;
                float length = direction.magnitude;

                puckLine.transform.position = midPoint;
                puckLine.transform.rotation = Quaternion.LookRotation(direction.normalized);
                puckLine.transform.localScale = new Vector3(0.05f, 0.05f, length);
            }
            else
            {
                Debug.LogWarning("[Visualizer] puckLine object is null during Update");
            }
        }
        private void UpdateInterceptVisual(Vector3 projectedPoint)
        {
            if (interceptTargetSphere != null)
            {
                interceptTargetSphere.transform.position = projectedPoint;
            }
        }
        //    void Update()
        //    {
        //        if (controlledPlayer == null)
        //            return;

        //        if (puckTransform == null)
        //        {

        //        }
        //        var puck = NetworkBehaviourSingleton<PuckManager>.Instance.GetPlayerPuck(NetworkBehaviourSingleton<PuckManager>.Instance.OwnerClientId);
        //        if (puck != null)
        //            puckTransform = puck.transform;
        //        Quaternion neutralRotation = Quaternion.LookRotation(
        //controlledPlayer.Team.Value == PlayerTeam.Red
        //    ? Vector3.forward    // face +Z (toward Blue)
        //    : Vector3.back,      // face -Z (toward Red)
        //Vector3.up);
        //        Vector3 neutralForward = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.forward : Vector3.back;
        //        if (body == null)
        //        {
        //            body = controlledPlayer.GetComponent<PlayerBodyV2>();
        //            if (body == null)
        //                return;
        //        }
        //        if (interceptTargetSphere == null)
        //        {
        //            interceptTargetSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //            interceptTargetSphere.name = "InterceptTargetSphere";
        //            interceptTargetSphere.GetComponent<Collider>().enabled = false;

        //            var mat = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        //            mat.color = new Color(1f, 0.5f, 0f, 0.7f); // orange translucent
        //            interceptTargetSphere.GetComponent<Renderer>().material = mat;

        //            interceptTargetSphere.transform.localScale = Vector3.one * 0.3f;
        //        }
        //        Vector3 goaliePos = controlledPlayer.PlayerBody.Rigidbody.transform.position;
        //        Vector3 puckPos = puckTransform?.position ?? Vector3.zero;

        //        // Flat direction to puck
        //        Vector3 toPuck = puckPos - goaliePos;
        //        toPuck.y = 0f; // prevent vertical tilt

        //        if (toPuck.sqrMagnitude > 0.01f)
        //        {
        //            // 2. Desired flat direction to puck
        //            Vector3 desiredDir = toPuck.normalized;

        //            // 3. Angle between neutral and desired
        //            float angleBetween = Vector3.SignedAngle(neutralForward, desiredDir, Vector3.up);

        //            // 4. Clamp to ±90 degrees
        //            float clampedAngle = Mathf.Clamp(angleBetween, -GoalieSettings.Instance.MaxRotationAngle, GoalieSettings.Instance.MaxRotationAngle);

        //            // 5. Create rotation from neutral rotated by clamped angle
        //            Quaternion targetRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up) * Quaternion.LookRotation(neutralForward);

        //            // 6. Smoothly rotate goalie toward this target
        //            body.transform.rotation = Quaternion.Slerp(
        //                body.transform.rotation,
        //                targetRotation,
        //                Time.deltaTime * GoalieSettings.Instance.RotationSpeed
        //            );
        //        }
        //        Vector3 goalCenter = (controlledPlayer.Team.Value == PlayerTeam.Red) ? redGoal : blueGoal;
        //        // Vector from puck to center of goal
        //        Vector3 puckToGoal = goalCenter - puckPos;
        //        Vector3 puckToGoalDir = puckToGoal.normalized;

        //        // 1. Get vector from puck to goalie
        //        Vector3 puckToGoalie = goaliePos - puckPos;

        //        // 2. Project goalie onto the puck-goal line
        //        float projectedLength = Vector3.Dot(puckToGoalie, puckToGoalDir);
        //        Vector3 projectedPoint = puckPos + puckToGoalDir * projectedLength;
        //        projectedPoint.y = body.transform.position.y;
        //        if (interceptTargetSphere != null)
        //        {
        //            interceptTargetSphere.transform.position = projectedPoint;
        //        }
        //        // 3. Get vector from goalie to that projected point (perpendicular to the line)
        //        Vector3 correctionVector = projectedPoint - goaliePos;

        //        // 4. Use cross product to determine lateral direction
        //        Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);
        //        float lateralDistance = correctionVector.magnitude;
        //        float signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);
        //        Vector3 goalieForward = neutralForward;
        //        Vector3 goalieToPuck = puckPos - goaliePos;

        //        float forwardDot = Vector3.Dot(goalieForward, goalieToPuck.normalized);

        //        // If puck is behind goalie, don't dash
        //        if (forwardDot < 0.5f)
        //        {
        //            return;
        //        }
        //        float angle = Vector3.Angle(controlledPlayer.transform.forward, toPuck);

        //        //Debug.Log($"Right vector: {controlledPlayer.transform.right}, LateralOffset: {lateralOffset:F2}");
        //        //Debug.Log($"[GoalieAI] HasDashed: {body.HasDashed}, Lateral Offset: {lateralOffset:F2}");
        //        bool cooldownActive = Time.time < lastDashTime + GoalieSettings.Instance.DashCooldown;
        //        bool gracePeriodActive = Time.time < lastDashStartTime + GoalieSettings.Instance.DashCancelGrace;
        //        if (lateralDistance > GoalieSettings.Instance.DashThreshold && !cooldownActive)
        //        {
        //            controlledPlayer.PlayerInput.Client_SlideInputRpc(true);
        //            if (signedLateralOffset < 0)
        //            {
        //                body.transform.rotation = neutralRotation;
        //                controlledPlayer.PlayerInput.Client_DashRightInputRpc();
        //                hasDashed = true;
        //            }
        //            else
        //            {
        //                body.transform.rotation = neutralRotation;
        //                controlledPlayer.PlayerInput.Client_DashLeftInputRpc();
        //                hasDashed = true;
        //            }
        //            lastDashTime = Time.time;
        //        }
        //        else if (hasDashed && Mathf.Abs(signedLateralOffset) <= GoalieSettings.Instance.CancelThreshold)
        //        {
        //            //controlledPlayer.PlayerInput.Client_SlideInputRpc(false);
        //            body.CancelDash();
        //            targetCancelPosition = projectedPoint;// Or a precise computed intercept point
        //            Debug.Log($"[GoalieAI] Perfect alignment, canceling dash. Offset = {signedLateralOffset:F2}");
        //        }
        //        if (!lineInitialized)
        //        {
        //            puckLine = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //            puckLine.name = "PuckInterceptLine";
        //            puckLine.GetComponent<Collider>().enabled = false;

        //            var renderer = puckLine.GetComponent<Renderer>();
        //            renderer.material = new Material(Shader.Find("Legacy Shaders/Diffuse"));
        //            renderer.material.color = new Color(0f, 1f, 1f, 0.5f); // translucent cyan

        //            //Debug.Log("[Visualizer] Created puck line cube.");
        //            lineInitialized = true;
        //        }

        //        if (puckLine != null)
        //        {
        //            Vector3 start = goalCenter;
        //            Vector3 end = puckPos;

        //            //Debug.Log($"[Visualizer] Goal Center: {start}, Puck Pos: {end}");

        //            Vector3 midPoint = (start + end) / 2f;
        //            Vector3 direction = end - start;
        //            float length = direction.magnitude;

        //            //Debug.Log($"[Visualizer] Midpoint: {midPoint}, Direction: {direction}, Length: {length}");

        //            puckLine.transform.position = midPoint;
        //            puckLine.transform.rotation = Quaternion.LookRotation(direction.normalized);
        //            puckLine.transform.localScale = new Vector3(0.05f, 0.05f, length);

        //            //Debug.Log($"[Visualizer] Line updated. Position: {puckLine.transform.position}, Rotation: {puckLine.transform.rotation.eulerAngles}, Scale: {puckLine.transform.localScale}");
        //        }
        //        else
        //        {
        //            Debug.LogWarning("[Visualizer] puckLine object is null during Update");
        //        }
        //    }
    }
}