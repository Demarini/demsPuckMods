using PuckAIPractice.Patches;
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
        private bool isPreparingDash = false;
        private Quaternion targetDashRotation;
        private float dashReadyThresholdDegrees = 5f;
        private Vector3 pendingDashDirection;
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
          
            Vector3 goalCenter = (controlledPlayer.Team.Value == PlayerTeam.Red) ? redGoal : blueGoal;
            Vector3 goalieToPuck = puckPos - goaliePos;
            Vector3 goalieForward = neutralForward;
            Vector3 netToPuck = puckPos - goalCenter;
            Vector3 netForward = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.back : Vector3.forward;
            float forwardDot = Vector3.Dot(neutralForward, netToPuck.normalized);
            if(forwardDot < .5f)
            {
                if(controlledPlayer.Team.Value == PlayerTeam.Red)
                {
                    SimulateDashHelper.IsBehindNetRed = true;
                }
                else
                {
                    SimulateDashHelper.IsBehindNetBlue = true;
                }
            }
            else
            {
                if (controlledPlayer.Team.Value == PlayerTeam.Red)
                {
                    SimulateDashHelper.IsBehindNetRed = true;
                }
                else
                {
                    SimulateDashHelper.IsBehindNetBlue = true;
                }
            }
            float maxAngleThisFrame = GoalieSettings.Instance.MaxRotationAngle;

            // Skip flattening if puck is behind net
            Vector3 goalRight = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.left : Vector3.right;
            bool isBehindNet = forwardDot < 0.5f;
            if (!isBehindNet)
            {
                float puckToGoalDist = Vector3.Distance(puckPos, goalCenter);
                float flattenStart = 6f;
                float flattenEnd = 2.5f;
                float t = Mathf.InverseLerp(flattenEnd, flattenStart, puckToGoalDist);
                maxAngleThisFrame = Mathf.Lerp(0f, GoalieSettings.Instance.MaxRotationAngle, t);
            }

            RotateTowardPuck(toPuck, neutralForward, isBehindNet, maxAngleThisFrame, goalCenter, goalRight);
            CreateArrow(ref netForwardArrow, Color.green);
            //CreateArrow(ref netToPuckArrow, Color.blue);
            Color dotColor = (forwardDot >= 0.5f) ? Color.green : Color.red;
            UpdateArrow(netForwardArrow, goalCenter, goalCenter + neutralForward * 4f, dotColor); // Green = Goalie forward
            //UpdateArrow(netToPuckArrow, goaliePos, puckPos, dotColor);                        // Blue = To puck
            Vector3 projectedPoint = GetProjectedInterceptClamped(goaliePos, puckPos, goalCenter, forwardDot, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir);
            if (controlledPlayer.Team.Value == PlayerTeam.Red)
            {
                SimulateDashHelper.SignedLateralOffsetRed = signedLateralOffset;
            }
            else
            {
                SimulateDashHelper.SignedLateralOffsetBlue = signedLateralOffset;
            }
            lastComputedIntercept = projectedPoint;
            
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

        private void RotateTowardPuck(
    Vector3 toPuck,
    Vector3 neutralForward,
    bool isBehindNet,
    float maxAngle,
    Vector3 goalCenter,
    Vector3 goalRight)
        {
            if (isBehindNet)
            {
                // Puck is behind the net — figure out which post to pin to (based on net center)
                Vector3 netToPuck = puckTransform.position - goalCenter;
                float side = Vector3.Dot(netToPuck, goalRight); // Left or right of net center

                Vector3 faceDir = (side < 0f) ? -goalRight : goalRight;

                body.transform.rotation = Quaternion.Slerp(
                    body.transform.rotation,
                    Quaternion.LookRotation(faceDir, Vector3.up),
                    Time.deltaTime * GoalieSettings.Instance.RotationSpeed
                );
                return;
            }

            if (toPuck.sqrMagnitude <= 0.01f) return;

            // Normal front-of-net rotation logic
            Vector3 desiredDir = toPuck.normalized;
            float angleBetween = Vector3.SignedAngle(neutralForward, desiredDir, Vector3.up);
            float clampedAngle = Mathf.Clamp(angleBetween, -maxAngle, maxAngle);
            Quaternion targetRotation = Quaternion.AngleAxis(clampedAngle, Vector3.up) * Quaternion.LookRotation(neutralForward);

            body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation,
                targetRotation,
                Time.deltaTime * GoalieSettings.Instance.RotationSpeed
            );
        }
        private void UpdateArrow(GameObject obj, Vector3 start, Vector3 end, Color dotColor)
        {
            Vector3 direction = end - start;
            Vector3 midPoint = start + direction / 2f;
            obj.transform.position = midPoint;

            obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            obj.transform.localScale = new Vector3(0.1f, direction.magnitude / 2f, 0.1f);
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = dotColor;
        }
        private GameObject netForwardArrow;
        private GameObject netToPuckArrow;
        private GameObject leftBoundArrow;
        private GameObject rightBoundArrow;

        private void CreateArrow(ref GameObject obj, Color color)
        {
            if (obj == null)
            {
                var mat = new Material(Shader.Find("Sprites/Default"));
                obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obj.transform.localScale = new Vector3(0.1f, 2f, 0.1f); // thin and long
                obj.GetComponent<Renderer>().material = mat;
                obj.GetComponent<Renderer>().material.color = color;
            }
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

            // Use same frame as puckToGoalRight from front-of-net mode
            Vector3 puckToGoal = goalCenter - puckPos;
            puckToGoalDir = puckToGoal.normalized;
            Vector3 puckToGoalRight = Vector3.Cross(Vector3.up, puckToGoalDir);
            Vector3 goalRight = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.left : Vector3.right; // now used consistently in both modes
            // If puck is behind the goalie or in a steep angle, lock to post
            if (forwardDot < 0.5f)
            {
                //Vector3 goalRight = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.left : Vector3.right;
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
            float maxDashRange = 2f;
            //if (lateralDistance > maxDashRange)
            //    finalPoint = goaliePos + puckToGoalRight * Mathf.Sign(signedLateralOffset) * 10f;
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