﻿using System.Collections.Generic;
using UnityEngine;

namespace SebCharCtrl
{
    public class SebPlatform : SebRaycast
    {
        public LayerMask passengerMask;

        public Vector3[] localWaypoints;
        private Vector3[] globalWaypoints;

        public float speed;
        public bool cyclic;
        public float waitTime;
        [Range(0, 2)]
        public float easeAmount;
        private int fromWaypointIndex;
        private float percentBetweenWaypoints;
        private float nextMoveTime;
        private List<PassengerMovement> passengerMovement;
        private Dictionary<Transform, SebController> passengerDictionary = new Dictionary<Transform, SebController>();

        public void Start()
        {
            //base.Start();

            globalWaypoints = new Vector3[localWaypoints.Length];
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                globalWaypoints[i] = localWaypoints[i] + transform.position;
            }
        }

        private void Update()
        {

            UpdateColliderBounds();

            Vector3 velocity = CalculatePlatformMovement();

            CalculatePassengerMovement(velocity);

            MovePassengers(true);
            transform.Translate(velocity);
            MovePassengers(false);
        }

        private float Ease(float x)
        {
            float a = easeAmount + 1;
            return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
        }

        private Vector3 CalculatePlatformMovement()
        {

            if (Time.time < nextMoveTime)
            {
                return Vector3.zero;
            }

            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;

                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        System.Array.Reverse(globalWaypoints);
                    }
                }
                nextMoveTime = Time.time + waitTime;
            }

            return newPos - transform.position;
        }

        private void MovePassengers(bool beforeMovePlatform)
        {
            foreach (PassengerMovement passenger in passengerMovement)
            {
                if (!passengerDictionary.ContainsKey(passenger.transform))
                {
                    passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<SebController>());
                }

                if (passenger.moveBeforePlatform == beforeMovePlatform)
                {
                    //passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
                }
            }
        }

        private void CalculatePassengerMovement(Vector3 velocity)
        {
            HashSet<Transform> movedPassengers = new HashSet<Transform>();
            passengerMovement = new List<PassengerMovement>();

            float directionX = Mathf.Sign(velocity.x);
            float directionY = Mathf.Sign(velocity.y);

            // Vertically moving platform
            if (velocity.y != 0)
            {
                float rayLength = Mathf.Abs(velocity.y) + SkinWidth;

                for (int i = 0; i < verRayCount; i++)
                {
                    Vector2 rayOrigin = (directionY == -1) ? CollBounds.bottomLeft : CollBounds.topLeft;
                    rayOrigin += Vector2.right * (verRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = (directionY == 1) ? velocity.x : 0;
                            float pushY = velocity.y - (hit.distance - SkinWidth) * directionY;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));
                        }
                    }
                }
            }

            // Horizontally moving platform
            if (velocity.x != 0)
            {
                float rayLength = Mathf.Abs(velocity.x) + SkinWidth;

                for (int i = 0; i < horRayCount; i++)
                {
                    Vector2 rayOrigin = (directionX == -1) ? CollBounds.bottomLeft : CollBounds.bottomRight;
                    rayOrigin += Vector2.up * (horRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x - (hit.distance - SkinWidth) * directionX;
                            float pushY = -SkinWidth;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));
                        }
                    }
                }
            }

            // Passenger on top of a horizontally or downward moving platform
            if (directionY == -1 || velocity.y == 0 && velocity.x != 0)
            {
                float rayLength = SkinWidth * 2;

                for (int i = 0; i < verRayCount; i++)
                {
                    Vector2 rayOrigin = CollBounds.topLeft + Vector2.right * (verRaySpacing * i);
                    RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);

                    if (hit && hit.distance != 0)
                    {
                        if (!movedPassengers.Contains(hit.transform))
                        {
                            movedPassengers.Add(hit.transform);
                            float pushX = velocity.x;
                            float pushY = velocity.y;

                            passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));
                        }
                    }
                }
            }
        }

        private struct PassengerMovement
        {
            public Transform transform;
            public Vector3 velocity;
            public bool standingOnPlatform;
            public bool moveBeforePlatform;

            public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool moveBeforePlatform)
            {
                this.transform = transform;
                this.velocity = velocity;
                this.standingOnPlatform = standingOnPlatform;
                this.moveBeforePlatform = moveBeforePlatform;
            }
        }

        private void OnDrawGizmos()
        {
            if (localWaypoints != null)
            {
                Gizmos.color = Color.red;
                float size = .3f;

                for (int i = 0; i < localWaypoints.Length; i++)
                {
                    Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                    Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                    Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
                }
            }
        }
    }
}