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
        private PlayerBodyV2 body;
        private Vector3? targetCancelPosition = null;
        private float cancelInterpSpeed = 20f; // tweak this value to control smoothness
        private bool isPreparingDash = false;
        private Vector3 pendingDashDir;
        private float dashReadyThreshold = 5f; // degrees
        bool hasDashed = false;
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
                    SimulateDashHelper.IsBehindNetRed = false;
                }
                else
                {
                    SimulateDashHelper.IsBehindNetBlue = false;
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
            if (!isPreparingDash)
            {
                RotateTowardPuck(toPuck, neutralForward, isBehindNet, maxAngleThisFrame, goalCenter, goalRight);
            }
            CreateArrow(ref netForwardArrow, Color.green);
            //CreateArrow(ref netToPuckArrow, Color.blue);
            Color dotColor = (forwardDot >= 0.5f) ? Color.green : Color.red;
            UpdateArrow(netForwardArrow, goalCenter, goalCenter + neutralForward * 4f, dotColor); // Green = Goalie forward
            //UpdateArrow(netToPuckArrow, goaliePos, puckPos, dotColor);                        // Blue = To puck
            Vector3 projectedPoint = GetProjectedInterceptClamped(goaliePos, puckPos, goalCenter, forwardDot, out float signedLateralOffset, out float lateralDistance, out Vector3 puckToGoalDir);
            if (controlledPlayer.Team.Value == PlayerTeam.Red)
            {
                SimulateDashHelper.ProjectedPointRed = projectedPoint;
            }
            else
            {
                SimulateDashHelper.ProjectedPointBlue = projectedPoint;
            }
            
            Debug.Log("Projected Point: " + projectedPoint);
            if (controlledPlayer.Team.Value == PlayerTeam.Red)
            {
                SimulateDashHelper.SignedLateralOffsetRed = signedLateralOffset;
            }
            else
            {
                SimulateDashHelper.SignedLateralOffsetBlue = signedLateralOffset;
            }
            lastComputedIntercept = projectedPoint;
            //Debug.Log($"[Update] Calling HandleDashLogic (isPreparingDash: {isPreparingDash})");
            //Debug.Log($"[NeutralRotation] {neutralRotation.eulerAngles}");
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

                // Desired board-facing direction (based on side of net)
                Vector3 desiredBoardDir = (side < 0f) ? -goalRight : goalRight;

                // Clamp angle around neutral forward
                float angleToBoard = Vector3.SignedAngle(neutralForward, desiredBoardDir, Vector3.up);
                float clampedAngle2 = Mathf.Clamp(angleToBoard, -maxAngle, maxAngle);

                Quaternion targetRotation2 = Quaternion.AngleAxis(clampedAngle2, Vector3.up) * Quaternion.LookRotation(neutralForward);

                // Smooth rotate
                body.transform.rotation = Quaternion.Slerp(
                    body.transform.rotation,
                    targetRotation2,
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

                //Debug.Log($"[Intercept] GoalRight: {goalRight}, Anchor: {anchor}, GoaliePos: {goaliePos}, SignedOffset: {signedLateralOffset}");

                return anchor;
            }

            puckToGoal = goalCenter - puckPos;

            // Avoid divide-by-zero
            if (Mathf.Abs(puckToGoal.z) < 0.001f)
                puckToGoal.z = 0.001f;

            // Time to reach goalie’s Z position on that line
            float t = (goaliePos.z - puckPos.z) / puckToGoal.z;

            // Projected point on puck-goal line at goalie’s Z
            Vector3 finalPoint = puckPos + puckToGoal * t;
            finalPoint.y = goaliePos.y;  // Stay level

            // Lateral offset = how far sideways goalie is from that line
            Vector3 correctionVector = finalPoint - goaliePos;
            signedLateralOffset = Vector3.Dot(correctionVector, puckToGoalRight);  // or goalRight

            // Optional: Clamp how far laterally we care
            //float maxLateral = 3.5f;
            signedLateralOffset = Mathf.Clamp(signedLateralOffset, -maxLateral, maxLateral);

            // Optional: for animation/movement
            lateralDistance = Mathf.Abs(signedLateralOffset);

            // Optional debug
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

            // Start sliding
            controlledPlayer.PlayerInput.Client_SlideInputRpc(true);

            if (!cooldownActive && lateralDistance > GoalieSettings.Instance.DashThreshold && !isPreparingDash)
            {
                Vector3 dashDir = signedLateralOffset < 0 ? Vector3.left : Vector3.right;
                Vector3 teamRight = (controlledPlayer.Team.Value == PlayerTeam.Red) ? Vector3.left : Vector3.right;
                Vector3 dashWorldDir = teamRight * Mathf.Sign(signedLateralOffset);

                pendingDashDir = dashWorldDir;   
                isPreparingDash = true;

                return;
            }
            else
            {
                //Debug.Log("Not ready for dashin yet bud!");
                //Debug.Log("Threshold:" + (lateralDistance > GoalieSettings.Instance.DashThreshold));
                //Debug.Log("Cooldown Active:" + cooldownActive);
            }

            // If we’re prepping to dash, wait until we’re aligned
            if (isPreparingDash)
            {
                body.transform.rotation = Quaternion.RotateTowards(
                    body.transform.rotation,
                    neutralRotation,
                    Time.deltaTime * GoalieSettings.Instance.RotationSpeed * 60f
                );

                float angle = Quaternion.Angle(body.transform.rotation, neutralRotation);
                if (angle <= dashReadyThreshold)
                {
                    // Now dash in correct direction
                    if ((pendingDashDir.x < 0 && controlledPlayer.Team.Value == PlayerTeam.Red) || (pendingDashDir.x >= 0 && controlledPlayer.Team.Value == PlayerTeam.Blue))
                    {
                        //Debug.Log($"{controlledPlayer.Username.Value} Dashed Left");
                        controlledPlayer.PlayerInput.Client_DashLeftInputRpc();
                        hasDashed = true;
                    }
                    else
                    {
                        //Debug.Log($"{controlledPlayer.Username.Value} Dashed Right");
                        controlledPlayer.PlayerInput.Client_DashRightInputRpc();
                        hasDashed = true;
                    }
                    lastDashTime = Time.time;
                    isPreparingDash = false;
                }


                return;
            }

            // If we’ve already dashed and we’re aligned, cancel dash
            if (Mathf.Abs(signedLateralOffset) <= GoalieSettings.Instance.CancelThreshold && hasDashed)
            {
                hasDashed = false; ;
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
                //Debug.LogWarning("[Visualizer] puckLine object is null during Update");
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