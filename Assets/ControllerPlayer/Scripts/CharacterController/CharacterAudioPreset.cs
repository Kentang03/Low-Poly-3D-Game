using UnityEngine;

namespace Invector.vCharacterController
{
    /// <summary>
    /// Scriptable Object untuk menyimpan preset audio settings
    /// Berguna untuk berbagi audio configuration antar multiple characters
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterAudioPreset", menuName = "Invector/Character Audio Preset", order = 1)]
    public class CharacterAudioPreset : ScriptableObject
    {
        [Header("Jump & Landing")]
        public AudioClip jumpSound;
        [Range(0f, 1f)] public float jumpVolume = 0.7f;
        public AudioClip landingSound;
        [Range(0f, 1f)] public float landingVolume = 0.6f;

        [Header("Walk Footsteps")]
        public AudioClip[] walkFootsteps;
        [Range(0f, 1f)] public float walkVolume = 0.5f;
        public float walkStepInterval = 0.5f;

        [Header("Run Footsteps")]
        public AudioClip[] runFootsteps;
        [Range(0f, 1f)] public float runVolume = 0.6f;
        public float runStepInterval = 0.35f;

        [Header("Sprint Footsteps")]
        public AudioClip[] sprintFootsteps;
        [Range(0f, 1f)] public float sprintVolume = 0.7f;
        public float sprintStepInterval = 0.25f;

        [Header("Advanced")]
        public float minInputForFootsteps = 0.1f;
        public bool useRandomPitch = true;
        [Range(0f, 0.3f)] public float pitchVariation = 0.1f;

        /// <summary>
        /// Apply this preset to a CharacterAudioController
        /// </summary>
        public void ApplyToController(CharacterAudioController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Cannot apply preset: controller is null");
                return;
            }

            // Apply jump & landing
            controller.jumpSound = jumpSound;
            controller.jumpVolume = jumpVolume;
            controller.landingSound = landingSound;
            controller.landingVolume = landingVolume;

            // Apply walk
            controller.walkFootsteps = walkFootsteps;
            controller.walkVolume = walkVolume;
            controller.walkStepInterval = walkStepInterval;

            // Apply run
            controller.runFootsteps = runFootsteps;
            controller.runVolume = runVolume;
            controller.runStepInterval = runStepInterval;

            // Apply sprint
            controller.sprintFootsteps = sprintFootsteps;
            controller.sprintVolume = sprintVolume;
            controller.sprintStepInterval = sprintStepInterval;

            // Apply advanced
            controller.minInputForFootsteps = minInputForFootsteps;
            controller.useRandomPitch = useRandomPitch;
            controller.pitchVariation = pitchVariation;

            Debug.Log($"Audio preset '{name}' applied to {controller.gameObject.name}");
        }

        /// <summary>
        /// Create a preset from an existing CharacterAudioController
        /// </summary>
        public void LoadFromController(CharacterAudioController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Cannot load from controller: controller is null");
                return;
            }

            // Load jump & landing
            jumpSound = controller.jumpSound;
            jumpVolume = controller.jumpVolume;
            landingSound = controller.landingSound;
            landingVolume = controller.landingVolume;

            // Load walk
            walkFootsteps = controller.walkFootsteps;
            walkVolume = controller.walkVolume;
            walkStepInterval = controller.walkStepInterval;

            // Load run
            runFootsteps = controller.runFootsteps;
            runVolume = controller.runVolume;
            runStepInterval = controller.runStepInterval;

            // Load sprint
            sprintFootsteps = controller.sprintFootsteps;
            sprintVolume = controller.sprintVolume;
            sprintStepInterval = controller.sprintStepInterval;

            // Load advanced
            minInputForFootsteps = controller.minInputForFootsteps;
            useRandomPitch = controller.useRandomPitch;
            pitchVariation = controller.pitchVariation;

            Debug.Log($"Loaded settings from {controller.gameObject.name} into preset '{name}'");
        }
    }
}
