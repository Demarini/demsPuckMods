using System.Collections;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace PuckAIPractice.AI
{
    public class GoalieAI : MonoBehaviour
    {
        Vector3 redGoal = new Vector3(0.0f, 0f, -40.23f);
        Vector3 blueGoal = new Vector3(0.0f, 0f, 40.23f);
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
        //void FixedUpdate()
        //{
        //    //if (targetCancelPosition.HasValue)
        //    //{
        //    //    Debug.Log("Cancelling Dash!");

        //    //    Vector3 current = body.transform.position;
        //    //    Vector3 target = targetCancelPosition.Value;
        //    //    Debug.Log(targetCancelPosition.Value);
        //    //    body.Rigidbody.MovePosition(targetCancelPosition.Value);
        //    //    targetCancelPosition = null;
        //    //    hasDashed = false;
        //    //    //// Smooth step toward cancel point
        //    //    //Vector3 next = Vector3.MoveTowards(current, target, cancelInterpSpeed * Time.fixedDeltaTime);

        //    //    //body.Rigidbody.MovePosition(next);

        //    //    //// Close enough? Snap and stop
        //    //    //if (Vector3.Distance(next, target) < 0.01f)
        //    //    //{
        //    //    //    body.Rigidbody.MovePosition(target);
        //    //    //    hasDashed = false;
        //    //    //    targetCancelPosition = null;
        //    //    //}
        //    //}
        //}
        Vector3 lastComputedIntercept;
        public Vector3 GetInterceptPoint()
        {
            return lastComputedIntercept; // or however you calculate it
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
            Vector3 goalieToPuck = puckPos - goaliePos;
            Vector3 goalieForward = neutralForward;
            float forwardDot = Vector3.Dot(goalieForward, goalieToPuck.normalized);
            Vector3 projectedPoint = GetProjectedInterceptClamped(goaliePos, puckPos, goalCenter, forwardDot, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir);
            lastComputedIntercept = projectedPoint;
            

           
            //if (forwardDot < .5f)
            //{
            //    projectedPoint = controlledPlayer.Team.Value == PlayerTeam.Blue ? blueGoal : redGoal;
            //    if(signedLateralOffset < 0)
            //    {
            //        //right
            //        //projectedPoint.x -= 2;
            //        //projectedPoint.z -= GoalieSettings.Instance.DistanceFromNet;
            //        //HandleDashLogic(toPuck, neutralRotation, lateralDistance, -signedLateralOffset, projectedPoint);
            //    }
            //    else
            //    {
            //        //projectedPoint.x += 2;
            //        //projectedPoint.z -= GoalieSettings.Instance.DistanceFromNet;
            //        //HandleDashLogic(toPuck, neutralRotation, lateralDistance, -signedLateralOffset, projectedPoint);
            //    }
            //    UpdateInterceptVisual(projectedPoint);
            //    //Debug.Log("Behind Net");
            //    return;
            //}
            //else
            //{
            //    //Debug.Log("Good Position");
            //}
            HandleDashLogic(toPuck, neutralRotation, lateralDistance, signedLateralOffset, projectedPoint);
            UpdateInterceptVisual(projectedPoint);
            UpdatePuckLine(goalCenter, puckPos);
        }
        private void ResolvePuckReference()
        {
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

            var mat = new Material(Shader.Find("Sprites/Default"));
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
        private Vector3 GetProjectedInterceptClampedPost(
    Vector3 goaliePos,
    Vector3 puckPos,
    Vector3 goalCenter,
    out float signedLateralOffset,
    out float lateralDistance,
    out Vector3 puckToGoalDir)
        {
            Vector3 puckToGoal = goalCenter - puckPos;
            puckToGoalDir = puckToGoal.normalized;

            Vector3 puckToGoalie = goaliePos - puckPos;
            float projectedLength = Vector3.Dot(puckToGoalie, puckToGoalDir);
            float forwardDot = Vector3.Dot((puckPos - goaliePos).normalized, puckToGoalDir);

            // Clamp range for projection
            projectedLength = Mathf.Clamp(projectedLength, 0f, 20f);

            Vector3 projectedPoint = puckPos + puckToGoalDir * projectedLength;
            projectedPoint.y = goaliePos.y;

            Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);
            Vector3 correctionVector = projectedPoint - goaliePos;

            signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);
            lateralDistance = correctionVector.magnitude;

            // If puck is behind the goal or coming in too steep (bad angle)
            if (forwardDot < 0.5f)
            {
                // Adjust these as needed
                float postOffset = 2.5f;  // How far left/right from center
                float goalDepthOffset = 1.0f; // How far forward (out of net)

                float puckXFromCenter = puckPos.x - goalCenter.x;
                float side = Mathf.Sign(puckXFromCenter);

                signedLateralOffset = side * postOffset;
                lateralDistance = Mathf.Abs(postOffset);

                // Base anchor from the *goal line*, not the goalie position
                Vector3 anchor = goalCenter + Vector3.right * signedLateralOffset;

                // Push forward toward the ice (out of net)
                anchor.z += (goalCenter.z > 0 ? -goalDepthOffset : goalDepthOffset);

                // Keep Y flat with the goalie
                anchor.y = goaliePos.y;

                if (interceptTargetSphere != null)
                    interceptTargetSphere.transform.position = anchor;

                return anchor;
            }

            // Normal intercept logic
            float maxLateral = 3.5f;
            signedLateralOffset = Mathf.Clamp(signedLateralOffset, -maxLateral, maxLateral);
            Vector3 finalOffset = puckToGoalRight * signedLateralOffset;

            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = goaliePos + finalOffset;

            return goaliePos + finalOffset;
        }
        private Vector3 GetProjectedInterceptOnGoalLine(
    Vector3 goaliePos,
    Vector3 puckPos,
    Vector3 goalCenter,
    PlayerTeam team,
    out float signedLateralOffset,
    out float lateralDistance,
    out Vector3 puckToGoalDir)
        {
            // Use team to define goalie-side direction
            Vector3 teamDirection = (team == PlayerTeam.Red) ? Vector3.back : Vector3.forward;
            Vector3 teamRight = Vector3.right;

            // Goalie line origin: a fixed distance in front of the goal, toward the ice
            float goalieDepth = GoalieSettings.Instance.DistanceFromNet;
            Vector3 goalieLineOrigin = goalCenter + teamDirection * goalieDepth;

            // Direction from puck to goal (used for sliding and aligning movement)
            puckToGoalDir = (goalCenter - puckPos).normalized;

            // Vector from the goal line origin to the puck
            Vector3 puckOffset = puckPos - goalieLineOrigin;

            // How far left/right the puck is from the center line (dot with "right" vector)
            float rawLateral = Vector3.Dot(puckOffset, teamRight);

            // Clamp to avoid going outside posts
            float maxLateral = 3.5f;
            float clampedLateral = Mathf.Clamp(rawLateral, -maxLateral, maxLateral);

            // Project lateral offset along goalie line
            Vector3 interceptOnLine = goalieLineOrigin + teamRight * clampedLateral;

            // Flatten to goalie height, do not move forward/back — goalie stays on line
            Vector3 intercept = new Vector3(interceptOnLine.x, goaliePos.y, goalieLineOrigin.z);

            // Outputs
            signedLateralOffset = clampedLateral;
            lateralDistance = (intercept - goaliePos).magnitude;

            // Optional debug sphere
            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = intercept;

            // Debug Logging
            Debug.Log($"[FixedGoalieLine]");
            Debug.Log($"  team              : {team}");
            Debug.Log($"  teamDirection     : {teamDirection}");
            Debug.Log($"  teamRight         : {teamRight}");
            Debug.Log($"  goalieLineOrigin  : {goalieLineOrigin}");
            Debug.Log($"  puckOffset        : {puckOffset}");
            Debug.Log($"  Raw Offset        : {rawLateral}");
            Debug.Log($"  Clamped Offset    : {clampedLateral}");
            Debug.Log($"  Intercept Point   : {intercept}");

            return intercept;
        }
        private Vector3 GetProjectedInterceptOnGoalLine(
    Vector3 goaliePos,
    Vector3 puckPos,
    Vector3 goalCenter,
    out float signedLateralOffset,
    out float lateralDistance,
    out Vector3 puckToGoalDir)
        {
            // Assume goal is on the Z axis — red goal at +Z, blue goal at -Z
            Vector3 teamDirection = (goalCenter.z > 0) ? Vector3.back : Vector3.forward; // Into the rink
            Vector3 teamRight = Vector3.right * ((goalCenter.x >= 0) ? 1 : -1); // Right is always global right

            // Set puckToGoalDir just for reference
            puckToGoalDir = teamDirection;

            // Fixed goalie line origin
            float goalieDepth = GoalieSettings.Instance.DistanceFromNet;
            Vector3 goalieLineOrigin = goalCenter + teamDirection * goalieDepth;

            // Project puck to this line to determine offset
            Vector3 puckOffset = puckPos - goalieLineOrigin;
            float rawLateral = Vector3.Dot(puckOffset, teamRight);

            float maxLateral = 3.5f;
            float clampedLateral = Mathf.Clamp(rawLateral, -maxLateral, maxLateral);

            Vector3 intercept = goalieLineOrigin + teamRight * clampedLateral;
            intercept.y = goaliePos.y;

            signedLateralOffset = clampedLateral;
            lateralDistance = Vector3.Distance(goaliePos, intercept);

            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = intercept;

            // Debug
            Debug.Log($"[FixedGoalieLine]");
            Debug.Log($"  teamDirection     : {teamDirection}");
            Debug.Log($"  teamRight         : {teamRight}");
            Debug.Log($"  goalieLineOrigin  : {goalieLineOrigin}");
            Debug.Log($"  puckOffset        : {puckOffset}");
            Debug.Log($"  Raw Offset        : {rawLateral}");
            Debug.Log($"  Clamped Offset    : {clampedLateral}");
            Debug.Log($"  Intercept Point   : {intercept}");

            return intercept;
        }
        //private Vector3 GetProjectedInterceptOnGoalLine(Vector3 goaliePos, Vector3 puckPos, Vector3 goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir)
        //{
        //    // Fixed forward direction based on team
        //    puckToGoalDir = (goalCenter - puckPos).normalized;

        //    Vector3 rinkRight = (goalCenter.z > 0) ? Vector3.right : Vector3.left; // Red net is at +Z
        //    Vector3 goalLineOrigin = goalCenter - puckToGoalDir * GoalieSettings.Instance.DistanceFromNet;

        //    Vector3 puckOffset = puckPos - goalLineOrigin;
        //    float lateralOffset = Vector3.Dot(puckOffset, rinkRight);

        //    float maxLateral = 3.5f;
        //    float clampedOffset = Mathf.Clamp(lateralOffset, -maxLateral, maxLateral);

        //    signedLateralOffset = clampedOffset;
        //    Vector3 interceptPoint = goalLineOrigin + rinkRight * clampedOffset;
        //    lateralDistance = (interceptPoint - goaliePos).magnitude;

        //    if (interceptTargetSphere != null)
        //        interceptTargetSphere.transform.position = interceptPoint;

        //    // Optional: still log with the simplified direction
        //    Debug.Log($"[InterceptCalc] puckOffset: {puckOffset}, raw: {lateralOffset}, clamped: {clampedOffset}, Intercept: {interceptPoint}");

        //    return interceptPoint;
        //}
        //private Vector3 GetProjectedInterceptOnGoalLine(Vector3 goaliePos, Vector3 puckPos, Vector3 goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir)
        //{
        //    puckToGoalDir = (goalCenter - puckPos).normalized;
        //    Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir); // Right along goal line

        //    // Where the goalie wants to project along the goal line, at a fixed depth
        //    Vector3 goalLineOrigin = goalCenter - puckToGoalDir * GoalieSettings.Instance.DistanceFromNet;
        //    Vector3 goaliePlanePos = goalLineOrigin;

        //    // Offset from puck to the goal projection plane
        //    Vector3 puckOffset = puckPos - goaliePlanePos;
        //    float lateralOffset = Vector3.Dot(puckOffset, puckToGoalRight);

        //    // Clamp lateral movement
        //    float maxLateral = 3.5f;
        //    float clampedOffset = Mathf.Clamp(lateralOffset, -maxLateral, maxLateral);

        //    signedLateralOffset = clampedOffset;
        //    Vector3 interceptPoint = goaliePlanePos + puckToGoalRight * clampedOffset;
        //    lateralDistance = (interceptPoint - goaliePos).magnitude;

        //    if (interceptTargetSphere != null)
        //        interceptTargetSphere.transform.position = interceptPoint;

        //    // 🔍 Logging
        //    Debug.Log($"[InterceptCalc] -------------------------");
        //    Debug.Log($"Goalie Pos        : {goaliePos}");
        //    Debug.Log($"Puck Pos          : {puckPos}");
        //    Debug.Log($"Goal Center       : {goalCenter}");
        //    Debug.Log($"puckToGoalDir     : {puckToGoalDir}");
        //    Debug.Log($"puckToGoalRight   : {puckToGoalRight}");
        //    Debug.Log($"goalLineOrigin    : {goalLineOrigin}");
        //    Debug.Log($"puckOffset        : {puckOffset}");
        //    Debug.Log($"Raw Offset        : {lateralOffset}");
        //    Debug.Log($"Clamped Offset    : {clampedOffset}");
        //    Debug.Log($"Intercept Point   : {interceptPoint}");
        //    Debug.Log($"Lateral Distance  : {lateralDistance}");

        //    return interceptPoint;
        //}
        //private Vector3 GetProjectedInterceptOnGoalLine(Vector3 goaliePos, Vector3 puckPos, Vector3 goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir)
        //{
        //    puckToGoalDir = (goalCenter - puckPos).normalized;
        //    Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir); // Right along goal line

        //    // Use lateral projection only — how far left/right the puck is from the center line
        //    Vector3 goalLineOrigin = goalCenter - puckToGoalDir * GoalieSettings.Instance.DistanceFromNet; // Fixed depth out from goal
        //    Vector3 goaliePlanePos = goalLineOrigin;

        //    Vector3 puckOffset = puckPos - goaliePlanePos;
        //    float lateralOffset = Vector3.Dot(puckOffset, puckToGoalRight);

        //    // Clamp so they don’t slide too far outside net width
        //    float maxLateral = 3.5f;
        //    lateralOffset = Mathf.Clamp(lateralOffset, -maxLateral, maxLateral);

        //    signedLateralOffset = lateralOffset;
        //    Vector3 interceptPoint = goaliePlanePos + puckToGoalRight * lateralOffset;
        //    lateralDistance = (interceptPoint - goaliePos).magnitude;

        //    if (interceptTargetSphere != null)
        //        interceptTargetSphere.transform.position = interceptPoint;

        //    return interceptPoint;
        //}
        private Vector3 GetProjectedInterceptClamped(
    Vector3 goaliePos,
    Vector3 puckPos,
    Vector3 goalCenter,
    float forwardDot,
    out float signedLateralOffset,
    out float lateralDistance,
    out Vector3 puckToGoalDir)
        {
            const float maxLateral = 3.5f;

            Vector3 puckToGoal = goalCenter - puckPos;
            puckToGoalDir = puckToGoal.normalized;
            Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);

            // If puck is behind the goalie or in a steep angle, lock to post
            if (forwardDot < 0.5f)
            {
                Vector3 goalRight = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.left : Vector3.right;
                Vector3 sideVec = puckPos - goalCenter;
                float side = Vector3.Dot(sideVec, goalRight); // Which side of the net

                float postOffset = 1.5f;        // How far from center to post
                float goalieDepth = -GoalieSettings.Instance.DistanceFromNet; ;       // How far out in front of net

                // Move left/right from center using world right
                Vector3 anchor = goalCenter + goalRight * Mathf.Sign(side) * postOffset;

                // Now pull the anchor forward out of the net
                anchor.z += (controlledPlayer.Team.Value == PlayerTeam.Red ? -goalieDepth : goalieDepth);

                // Flatten anchor
                anchor.y = goaliePos.y;

                // Signed offset = how far right from goalie (world space)
                signedLateralOffset = Vector3.Dot(anchor - goaliePos, goalRight);
                lateralDistance = Mathf.Abs(signedLateralOffset);

                if (interceptTargetSphere != null)
                    interceptTargetSphere.transform.position = anchor;

                Debug.Log($"[Intercept] GoalRight: {goalRight}, Anchor: {anchor}, GoaliePos: {goaliePos}, SignedOffset: {signedLateralOffset}");

                return anchor;
            }

            // Otherwise, calculate dynamic intercept normally
            Vector3 puckToGoalie = goaliePos - puckPos;
            float projectedLength = Vector3.Dot(puckToGoalie, puckToGoalDir);
            projectedLength = Mathf.Clamp(projectedLength, 0f, 60f);

            Vector3 projectedPoint = puckPos + puckToGoalDir * projectedLength;
            projectedPoint.y = goaliePos.y;

            Vector3 correctionVector = projectedPoint - goaliePos;
            signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);
            signedLateralOffset = Mathf.Clamp(signedLateralOffset, -maxLateral, maxLateral);

            Vector3 finalOffset = puckToGoalRight * signedLateralOffset;
            Vector3 finalPoint = goaliePos + finalOffset;

            lateralDistance = finalOffset.magnitude;

            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = finalPoint;

            return finalPoint;
        }
        private Vector3 GetProjectedInterceptClamped(Vector3 goaliePos, Vector3 puckPos, Vector3 goalCenter, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir)
        {
            Vector3 puckToGoal = goalCenter - puckPos;
            puckToGoalDir = puckToGoal.normalized;

            Vector3 puckToGoalie = goaliePos - puckPos;
            float projectedLength = Vector3.Dot(puckToGoalie, puckToGoalDir);

            // Optional: Don't project if behind or far away
            projectedLength = Mathf.Clamp(projectedLength, 0f, 60f);

            Vector3 projectedPoint = puckPos + puckToGoalDir * projectedLength;
            projectedPoint.y = goaliePos.y;

            Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);
            Vector3 correctionVector = projectedPoint - goaliePos;

            signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);
            lateralDistance = correctionVector.magnitude;

            // Clamp lateral movement
            float maxLateral = 3.5f;
            signedLateralOffset = Mathf.Clamp(signedLateralOffset, -maxLateral, maxLateral);
            Vector3 finalOffset = puckToGoalRight * signedLateralOffset;

            if (interceptTargetSphere != null)
                interceptTargetSphere.transform.position = goaliePos + finalOffset;

            return goaliePos + finalOffset;
        }

        private void HandleDashLogic(Vector3 toPuck, Quaternion neutralRotation, float lateralDistance, float signedLateralOffset, Vector3 projectedPoint)
        {
            bool cooldownActive = Time.time < lastDashTime + GoalieSettings.Instance.DashCooldown;
            bool gracePeriodActive = Time.time < lastDashStartTime + GoalieSettings.Instance.DashCancelGrace;
            controlledPlayer.PlayerInput.Client_SlideInputRpc(true);
            if (lateralDistance > GoalieSettings.Instance.DashThreshold && !cooldownActive)
            {
                body.transform.rotation = neutralRotation;
                Debug.Log("Wants to Dash");
                if (signedLateralOffset < 0)
                {
                    Debug.Log($"{controlledPlayer.Username.Value} Dashed Right");
                    Debug.Log($"Is Sliding {controlledPlayer.PlayerInput.SlideInput.ClientValue} "); 
                    controlledPlayer.PlayerInput.Client_DashRightInputRpc();
                    hasDashed = true;
                }
                else
                {
                    Debug.Log(signedLateralOffset);
                    Debug.Log($"{controlledPlayer.Username.Value} Dashed Left");
                    Debug.Log($"Is Sliding {controlledPlayer.PlayerInput.SlideInput.ClientValue} ");
                    controlledPlayer.PlayerInput.Client_DashLeftInputRpc();
                    hasDashed = true;
                }  
                lastDashTime = Time.time;
            }
            else if (hasDashed && Mathf.Abs(signedLateralOffset) <= GoalieSettings.Instance.CancelThreshold)
            {
                body.CancelDash();
                hasDashed = false;
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
                renderer.material = new Material(Shader.Find("Sprites/Default"));
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
    }
}