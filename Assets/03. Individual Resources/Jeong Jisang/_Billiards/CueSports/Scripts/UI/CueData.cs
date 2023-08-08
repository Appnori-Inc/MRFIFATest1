using UnityEngine;
using System.Collections;


namespace Billiards
{
    using UnityEditor;

    public class CueData : MonoBehaviour
    {
        public enum DataType
        {
            None,
            TableCenterTransform,
            TableCenterCameraSlot,
            StandardCameraSlot,
            LeftEye,
            CueBallCameraSlot,
            CueBallCameraRoot,
            StandardCueBallCameraSlot,
            WorldTempCameraSlot,
        }


        [SerializeField]
        private DataType myType;

        private void OnEnable()
        {
            switch (myType)
            {
                case DataType.None:
                    break;

                case DataType.TableCenterTransform:
                    BilliardsDataContainer.Instance.TableCameraCenter.Value = transform;
                    break;

                case DataType.TableCenterCameraSlot:
                    BilliardsDataContainer.Instance.TableCameraSlot.Value = transform;
                    break;

                case DataType.StandardCameraSlot:
                    BilliardsDataContainer.Instance.StandardCameraSlot.Value = transform;
                    break;

                case DataType.LeftEye:
                    BilliardsDataContainer.Instance.MainCamera.Value = GetComponent<Camera>();
                    break;

                case DataType.CueBallCameraSlot:
                    BilliardsDataContainer.Instance.CueBallCameraSlot.Value = transform;
                    break;

                case DataType.CueBallCameraRoot:
                    BilliardsDataContainer.Instance.CueBallCameraRootFollower.Value = GetComponent<FollowPosition>();
                    break;

                case DataType.StandardCueBallCameraSlot:
                    BilliardsDataContainer.Instance.StandardCueBallCameraSlot.Value = transform;
                    break;

                case DataType.WorldTempCameraSlot:
                    BilliardsDataContainer.Instance.WorldTempCameraSlot.Value = transform;
                    break;

                default:
                    break;
            }
        }
    }

}