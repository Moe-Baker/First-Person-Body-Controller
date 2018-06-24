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
                return Mathf.InverseLerp(130f * 4f, 0f, Mathf.Abs(freeLookRotation)) * 80f;
            }
        }
        public void ApplyVerticalRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);

            SetVerticalRotation(neck, value / 2f);
            SetVerticalRotation(head, value / 2f);
        }
        void SetVerticalRotation(Transform transform, float value)
        {
            var localAngles = transform.localEulerAngles;

            localAngles.z = value;

            transform.localEulerAngles = localAngles;
        }

        float freeLookRotation = 0f;
        void ApplyFreeLookRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            neck.Rotate(Vector3.right, value / 3f, Space.Self);
            spine.Rotate(Vector3.right, value / 3f, Space.Self);
            chest.Rotate(Vector3.right, value / 3f, Space.Self);
        }

        float leanAngle = 0f;
        void ApplyLeanAngle(float value)
        {
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);

            spine.Rotate(transform.up * value / 2f);
            chest.Rotate(transform.up * value / 2f);

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


            verticalRotation = Mathf.Clamp(verticalRotation + Input.GetAxis("Mouse Y") * sensitivity, -VerticallookLimit, VerticallookLimit);
            ApplyVerticalRotation(verticalRotation);

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
        }
    }
}