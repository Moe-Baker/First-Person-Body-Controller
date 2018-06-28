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
        Vector3 VerticalVector = Vector3.right;
        public void ApplyVerticalRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);

            int count = value < 0f ? 5 : 4;

            neck.Rotate(VerticalVector, value / count, Space.Self);
            head.Rotate(VerticalVector, value / count, Space.Self);
            chest.Rotate(VerticalVector, value / count, Space.Self);
            upperChest.Rotate(VerticalVector, value / count, Space.Self);
            if (value < 0f) spine.Rotate(VerticalVector, value / count, Space.Self);

            var rightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            var rightElbow = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);

            var leftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            var leftElbow = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);

            if(value < 0f)
            {
                rightArm.Rotate(chest.right, -value / 4, Space.World);

                leftArm.Rotate(chest.right, -value / 4, Space.World);
            }
            else
            {
                rightArm.Rotate(chest.right, -value / 2, Space.World);
                rightElbow.Rotate(chest.right, -value / 2 / 2, Space.World);

                leftArm.Rotate(chest.right, -value / 2, Space.World);
                leftElbow.Rotate(chest.right, -value / 2 / 2, Space.World);
            }
        }

        float freeLookRotation = 0f;
        Vector3 FreeLookVector = Vector3.up;
        void ApplyFreeLookRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);

            var count = 5;

            neck.Rotate(FreeLookVector, value / count, Space.Self);
            head.Rotate(FreeLookVector, value / count, Space.Self);
            spine.Rotate(FreeLookVector, value / count, Space.Self);
            chest.Rotate(FreeLookVector, value / count, Space.Self);
            upperChest.Rotate(FreeLookVector, value / count, Space.Self);
        }

        float leanAngle = 0f;
        Vector3 LeanVector = Vector3.forward;
        void ApplyLeanAngle(float value)
        {
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var upperChest = animator.GetBoneTransform(HumanBodyBones.UpperChest);

            var count = 3;

            spine.Rotate(LeanVector, value / count, Space.Self);
            chest.Rotate(LeanVector, value / count, Space.Self);
            upperChest.Rotate(LeanVector, value / count, Space.Self);

            var leftArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            var RightArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            if(value != 0f)
            {
                if (Mathf.Sign(value) > 0) // Left
                    leftArm.Rotate(Vector3.forward * -value);
                else // Right
                    RightArm.Rotate(Vector3.forward * -value);
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

            //Lean
            if (Input.GetKey(KeyCode.E))
                leanAngle = Mathf.MoveTowards(leanAngle, -50f, 150 * Time.deltaTime);
            else if (Input.GetKey(KeyCode.Q))
                leanAngle = Mathf.MoveTowards(leanAngle, 50f, 150 * Time.deltaTime);
            else
                leanAngle = Mathf.MoveTowards(leanAngle, 0f, 150 * Time.deltaTime);

            ApplyFreeLookRotation(freeLookRotation);


            //Freelook
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                freeLookRotation = Mathf.Clamp(freeLookRotation + Input.GetAxis("Mouse X") * sensitivity, -150f, 150f);
            }
            else
            {
                freeLookRotation = Mathf.MoveTowards(freeLookRotation, 0f, 360f * Time.deltaTime);

                transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);
            }
            ApplyLeanAngle(leanAngle);

            
            //Vertical Look
            verticalRotation = Mathf.Clamp(verticalRotation + -Input.GetAxis("Mouse Y") * sensitivity, -VerticallookLimit, VerticallookLimit);
            ApplyVerticalRotation(verticalRotation);
        }
    }
}