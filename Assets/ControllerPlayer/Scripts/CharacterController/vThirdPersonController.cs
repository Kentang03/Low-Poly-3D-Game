using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {
        public virtual void ControlAnimatorRootMotion()
        {
            if (!this.enabled || stopMove) return; // Tambahkan check stopMove

            // Hanya apply root motion jika ada input atau sedang bergerak
            if (inputSmooth == Vector3.zero && input == Vector3.zero)
            {
                // Jika benar-benar diam, jangan ubah position dan rotation dari root motion
                // Biarkan animator handle idle state secara natural tanpa mempengaruhi transform
                return;
            }

            if (inputSmooth == Vector3.zero)
            {
                transform.position = animator.rootPosition;
                // Jangan ubah rotation jika ada collision (stopMove = true)
                if (!stopMove)
                    transform.rotation = animator.rootRotation;
            }

            if (useRootMotion && !stopMove) // Tambahkan check stopMove
                MoveCharacter(moveDirection);
        }

        public virtual void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
            {
                SetControllerMoveSpeed(freeSpeed);
                SetAnimatorMoveSpeed(freeSpeed);
            }
            else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
            {
                isStrafing = true;
                SetControllerMoveSpeed(strafeSpeed);
                SetAnimatorMoveSpeed(strafeSpeed);
            }

            if (!useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlRotationType()
        {
            if (lockRotation || stopMove) return; // Tambahkan check stopMove untuk mencegah rotasi saat collision

            // Langsung reset inputSmooth jika tidak ada input untuk menghentikan rotasi lebih cepat
            if (input == Vector3.zero)
            {
                inputSmooth = Vector3.zero;
                return; // Langsung return jika tidak ada input
            }

            // Hanya rotate jika ada input aktual dari player
            bool hasActiveInput = input != Vector3.zero;
            bool shouldRotateWithCamera = (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera) && rotateTarget;
            
            // Validasi input - hanya rotate jika benar-benar ada input
            bool validInput = hasActiveInput || (shouldRotateWithCamera && hasActiveInput);

            if (validInput)
            {
                // calculate input smooth
                inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

                Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
                RotateToDirection(dir);
            }
        }

        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                // Langsung set ke zero untuk menghentikan movement lebih cepat
                moveDirection = Vector3.zero;
                inputSmooth = Vector3.zero;
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.right;
                right.y = 0;
                //get the forward direction relative to referenceTransform Right
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
                !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (isGrounded && useContinuousSprint)
                    {
                        isSprinting = !isSprinting;
                    }
                    else if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (!useContinuousSprint && isSprinting)
                {
                    isSprinting = false;
                }
            }
            else if (isSprinting)
            {
                isSprinting = false;
            }
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;

            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }
    }
}