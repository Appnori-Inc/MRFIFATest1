using Billiards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace BallPool.Mechanics
{
    public class Geometry
    {
        public static Vector3 ClampPositionInCube(Vector3 position, float radius, Transform cube)
        {
            if (SphereInCube(position, radius, cube))
            {
                return position;
            }
            else
            {
                Vector3 localPosition = GetLocalPosition(position, cube);
                float x = Mathf.Clamp(localPosition.x, -0.5f * cube.lossyScale.x + radius, 0.5f * cube.lossyScale.x - radius);
                float y = Mathf.Clamp(localPosition.y, -0.5f * cube.lossyScale.y + radius, 0.5f * cube.lossyScale.y - radius);
                float z = Mathf.Clamp(localPosition.z, -0.5f * cube.lossyScale.z + radius, 0.5f * cube.lossyScale.z - radius);
                Vector3 clampedLocalPosition = new Vector3(x, y, z);
                return GetWorldPosition(clampedLocalPosition, cube);
            }
        }

        public static Vector2 EdgeProjectionXZ(Vector2 direction, Vector2 pivot, Transform cube)
        {
            Vector2 cubeSizeXZ = cube.lossyScale.ToXZ();
            Vector2 result = direction * 100f;

            Vector2 localPivot = GetLocalPosition(pivot.ToVector3FromXZ(), cube).ToXZ();

            float slope = direction.y / direction.x;

            //First axis check
            result.y = Mathf.Clamp(result.y + localPivot.y, cubeSizeXZ.y * -0.5f, cubeSizeXZ.y * 0.5f);
            result.x = (Mathf.Abs(slope) <= 0.001f ? result.x : ((result.y - localPivot.y) / slope)) + localPivot.x;

            //Second axis check
            if (Mathf.Abs(result.x) > cubeSizeXZ.x * 0.5f)
            {
                result.x = Mathf.Clamp(result.x, cubeSizeXZ.x * -0.5f, cubeSizeXZ.x * 0.5f);
                result.y = (result.x - localPivot.x) * slope + localPivot.y;
            }

            return result;
        }

        public static bool SphereInCube(Vector3 position, float radius, Transform cube)
        {
            Vector3 localPos = GetLocalPosition(position, cube);
            Vector3 cubeScale = cube.lossyScale;
            return Mathf.Abs(localPos.x) + radius - 0.5f * cubeScale.x <= 0.0f &&
            Mathf.Abs(localPos.y) + radius - 0.5f * cubeScale.y <= 0.0f &&
            Mathf.Abs(localPos.z) + radius - 0.5f * cubeScale.z <= 0.0f;
        }

        public static Vector3 GetLocalPosition(Vector3 worldPosition, Transform shape)
        {
            Vector3 deltaPos = worldPosition - shape.position;
            return new Vector3(Vector3.Dot(deltaPos, shape.right), Vector3.Dot(deltaPos, shape.up), Vector3.Dot(deltaPos, shape.forward));
        }

        public static Vector3 GetWorldPosition(Vector3 localPosition, Transform shape)
        {
            return shape.position + localPosition.x * shape.right + localPosition.y * shape.up + localPosition.z * shape.forward;
        }
        public static Vector3 getPerpendicularToVector(Vector3 vector, Vector3 origin)
        {
            return origin - Vector3.Project(origin, vector);
        }
    }
}
