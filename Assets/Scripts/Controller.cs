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
                return Mathf.InverseLerp(130f, 0f, Mathf.Abs(freeLookRotation)) * 80f;
            }
        }
        public void ApplyVerticalRotation(float value)
        {
            var spine = animator.GetBoneTransform(HumanBodyBones.Spine);
            var chest = animator.GetBoneTransform(HumanBodyBones.Chest);
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);
            var head = animator.GetBoneTransform(HumanBodyBones.Head);

            SetRotation(neck, value / 2f);
            SetRotation(head, value / 2f);
            return;
            SetRotation(spine, value / 3f);
            SetRotation(chest, value / 3f);
        }
        void SetRotation(Transform transform, float value)
        {
            var localAngles = transform.localEulerAngles;

            localAngles.z = value;

            transform.localEulerAngles = localAngles;
        }

        float freeLookRotation = 0f;
        void ApplyFreeLookRotation(float value)
        {
            var neck = animator.GetBoneTransform(HumanBodyBones.Neck);

            neck.Rotate(Vector3.right, value, Space.Self);
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
            hips.localPosition = hipsPosition;
            hips.localRotation = hipsRotation;


            verticalRotation = Mathf.Clamp(verticalRotation + Input.GetAxis("Mouse Y") * sensitivity, -VerticallookLimit, VerticallookLimit);
            ApplyVerticalRotation(verticalRotation);

            if(Input.GetKey(KeyCode.LeftAlt))
            {
                freeLookRotation = Mathf.Clamp(freeLookRotation + -Input.GetAxis("Mouse X") * sensitivity, -130f, 130f);
            }
            else
            {
                freeLookRotation = Mathf.MoveTowards(freeLookRotation, 0f, 360f * Time.deltaTime);

                transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sensitivity);
            }
            ApplyFreeLookRotation(freeLookRotation);
        }
    }
}