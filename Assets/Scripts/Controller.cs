using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
	public class Controller : MonoBehaviour
	{
        public Animator animator;
        public CharacterController Character;

        [Space]
        new public Camera camera;

        public float sensitivity = 4f;

        float verticalRotation = 0f;
        float VerticallookLimit
        {
            get
            {
                return Mathf.InverseLerp(130f * 2f, 0f, Mathf.Abs(freeLookRotation)) * 80f;
            }
        }
        Vector3 VerticalVector = Vector3.forward;
        public void ApplyVerticalRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);

            neck.Rotate(VerticalVector, value / 2f, Space.Self);
            head.Rotate(VerticalVector, value / 2f, Space.Self);
        }

        float freeLookRotation = 0f;
        Vector3 FreeLookVector = Vector3.right;
        void ApplyFreeLookRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            float count = 4;

            neck.Rotate(FreeLookVector, value / count, Space.Self);
            head.Rotate(FreeLookVector, value / count, Space.Self);
            spine.Rotate(FreeLookVector, value / count, Space.Self);
            chest.Rotate(FreeLookVector, value / count, Space.Self);
        }

        float leanAngle = 0f;
        Vector3 LeanVector = Vector3.up;
        void ApplyLeanAngle(float value)
        {
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            spine.Rotate(LeanVector, value / 3f, Space.Self);
            chest.Rotate(LeanVector, value / 3f, Space.Self);

            var leftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            var RightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            if(value != 0f)
            {
                if (Mathf.Sign(value) > 0) // Left
                    leftArm.Rotate(transform.up * value);
                else // Right
                    RightArm.Rotate(transform.up * value);
            }
        }

        Transform hips;
        Vector3 hipsPosition;
        Quaternion hipsRotation;

        void Awake()
        {
            hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            hipsPosition = hips.localPosition;
            hipsRotation = hips.localRotation;
        }

        void Update()
        {
            animator.SetFloat("Vertical", Input.GetAxis("Vertical"), 0.02f, Time.deltaTime);
            animator.SetFloat("Horizontal", Input.GetAxis("Horizontal"), 0.02f, Time.deltaTime);


            Character.SimpleMove(animator.velocity);
        }

        void LateUpdate()
        {
            hips.localRotation = hipsRotation;

            if(Input.GetKey(KeyCode.LeftAlt))
            {
                freeLookRotation = Mathf.Clamp(freeLookRotation + -Input.GetAxis("Mouse X") * sensitivity, -150f, 150f);
            }
            else
            {
                freeLookRotation = Mathf.MoveTowards(freeLookRotation, 0f, 360f * Time.deltaTime);

                transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);
            }
            ApplyFreeLookRotation(freeLookRotation);

            if (Input.GetKey(KeyCode.E))
                leanAngle = Mathf.MoveTowards(leanAngle, -50f, 150 * Time.deltaTime);
            else if (Input.GetKey(KeyCode.Q))
                leanAngle = Mathf.MoveTowards(leanAngle, 50f, 150 * Time.deltaTime);
            else
                leanAngle = Mathf.MoveTowards(leanAngle, 0f, 150 * Time.deltaTime);

            ApplyLeanAngle(leanAngle);

            verticalRotation = Mathf.Clamp(verticalRotation + Input.GetAxis("Mouse Y") * sensitivity, -VerticallookLimit, VerticallookLimit);
            ApplyVerticalRotation(verticalRotation);
        }
    }
}