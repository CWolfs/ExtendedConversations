using UnityEngine;

using System.Linq;
using System.Collections.Generic;

using BattleTech;

using ExtendedConversations.Core;

namespace ExtendedConversations.Animation {
  public class AnimationManager {
    private static AnimationManager instance;
    public static AnimationManager Instance {
      get {
        if (instance == null) instance = new AnimationManager();
        return instance;
      }
    }

    public Dictionary<string, Dictionary<SimGameRoom, SimGameCharacter>> DisabledVanillaCrewMembers = new Dictionary<string, Dictionary<SimGameRoom, SimGameCharacter>>();  // <CrewMember Name, <Room, Character>>
    public Dictionary<string, Dictionary<SimGameRoom, SimGameCharacter>> MimicCrewMembers = new Dictionary<string, Dictionary<SimGameRoom, SimGameCharacter>>();  // <CrewMember Name, <Room, Character>>

    public void ReplaceAllVanillaCrewWithMimics() {
      Main.Logger.LogDebug("[AnimationManager.ReplaceAllVanillaCrewWithMimics] Replacing all crew members in the SimGame with mimics");

      SimGameCameraController simGameCameraController = UnityGameInstance.Instance.Game.Simulation.CameraController;
      SimGameRoom[] argoSimGameRooms = simGameCameraController.simGameRoomPrefabs;

      foreach (SimGameRoom room in argoSimGameRooms) {
        Main.Logger.Log("[Room] Argo Name: " + room.name);
      }

      SimGameRoom[] leopardSimGameRooms = simGameCameraController.leopardRoomPrefabs;

      foreach (SimGameRoom room in leopardSimGameRooms) {
        Main.Logger.Log("[Room] Leop Name: " + room.name);
      }

      // Argo: Conference Room

      // Argo: Command Center
      SimGameRoom simGameRoom = argoSimGameRooms.Where(room => room.name == SimGameRoomGameObjectName.ARGO_COMMAND_CENTER).FirstOrDefault();
      if (simGameRoom != null) {
        Main.Logger.Log($"[AnimationManager.ReplaceAllVanillaCrewWithMimics] Found room for Argo command center named '{simGameRoom.name}'");
        List<SimGameCharacter> charactersInRoom = simGameRoom.CharacterList;

        foreach (SimGameCharacter character in charactersInRoom) {
          if (character.GetComponent<Animator>()) {
            Main.Logger.Log("[AnimationManager.ReplaceAllVanillaCrewWithMimics] Character in room: " + character.name);
            string crewName = character.name.Substring(character.name.LastIndexOf("_") + 1);
            crewName = crewName.UpperFirst();
            Main.Logger.Log("[AnimationManager.ReplaceAllVanillaCrewWithMimics] Trimmed name is: " + crewName);

            SimGameCharacter mimicCharacter = ReplaceVanillaCrewMemberWithMimic(crewName, "IdleCalmMale", simGameRoom);
            if (!MimicCrewMembers.ContainsKey(crewName)) MimicCrewMembers.Add(crewName, new Dictionary<SimGameRoom, SimGameCharacter>());
            MimicCrewMembers[crewName][simGameRoom] = mimicCharacter;
          }
        }
      } else {
        Main.Logger.Log($"[AnimationManager.ReplaceAllVanillaCrewWithMimics] Couldn't find Argo command center '{simGameRoom.name}'");
      }

      // Argo: Engineering

      // Argo: Navigation

      // Argo: Mechbay

      // Leopard: Conference Room

      // Leopard: Command Center

      // Leopard: Engineering (does leopard have engineering? Need to check - can't remember)

      // Leopard: Navigation

      // Leopard: Mechbay
    }

    public SimGameCharacter ReplaceVanillaCrewMemberWithMimic(string crewName, string animationControllerName, SimGameRoom room) {
      GameObject crewMemberPrefab = AssetBundleLoader.GetAsset<GameObject>("ec-assets-bundle", $"EC" + crewName);
      RuntimeAnimatorController runtimeAnimatorController = AssetBundleLoader.GetAsset<RuntimeAnimatorController>("ec-assets-bundle", $"{animationControllerName}TestAnimationController");

      if (runtimeAnimatorController == null) {
        Main.Logger.LogError($"[Actions.TriggerCustomAnimation] '{animationControllerName}TestAnimationController' was not found in bundle");
        return null;
      }

      if (crewMemberPrefab == null) {
        Main.Logger.LogError($"[Actions.TriggerCustomAnimation] 'EC{crewName}' was not found in bundle");
        return null;
      }

      SimGameCameraController cameraController = UnityGameInstance.Instance.Game.Simulation.CameraController;
      List<SimGameCharacter> characterList = room.CharacterList;

      foreach (SimGameCharacter originalCharacter in characterList) {
        if (originalCharacter.character.ToString().ToLower().Contains(crewName.ToLower())) {
          Animator animator = originalCharacter.gameObject.GetComponent<Animator>();
          if (animator != null) {
            GameObject crewMemberGo = GameObject.Instantiate(crewMemberPrefab, originalCharacter.gameObject.transform.position, originalCharacter.gameObject.transform.rotation, originalCharacter.gameObject.transform.parent);
            crewMemberGo.transform.localScale = new Vector3(3.4f, 3.4f, 3.4f);

            CapsuleCollider originalCapsuleCollider = originalCharacter.gameObject.GetComponent<CapsuleCollider>();
            CapsuleCollider capsuleCollider = crewMemberGo.AddComponent<CapsuleCollider>();
            capsuleCollider.center = originalCapsuleCollider.center;
            capsuleCollider.radius = originalCapsuleCollider.radius;
            capsuleCollider.height = originalCapsuleCollider.height;
            capsuleCollider.direction = originalCapsuleCollider.direction;
            capsuleCollider.contactOffset = originalCapsuleCollider.contactOffset;

            crewMemberGo.name = crewMemberGo.name.Replace("(Clone)", "");
            if (originalCharacter.gameObject.activeSelf) originalCharacter.gameObject.SetActive(false);

            DressCharacter(originalCharacter.gameObject, crewMemberGo);
            Animator currentAnimator = crewMemberGo.GetComponent<Animator>();
            currentAnimator.applyRootMotion = false;
            currentAnimator.runtimeAnimatorController = runtimeAnimatorController;

            return crewMemberGo.CopyComponent(originalCharacter);
          }
        }
      }

      return null;
    }

    public void TriggerCustomAnimation(string crewName, string animationName, bool enableRootMotion) {
      RuntimeAnimatorController runtimeAnimatorController = AssetBundleLoader.GetAsset<RuntimeAnimatorController>("ec-assets-bundle", $"{animationName}TestAnimationController");

      if (runtimeAnimatorController == null) {
        Main.Logger.LogError($"[Actions.TriggerCustomAnimation] '{animationName}TestAnimationController' was not found in bundle");
        return;
      }

      SimGameCameraController cameraController = UnityGameInstance.Instance.Game.Simulation.CameraController;
      List<SimGameCharacter> characterList = cameraController.CurrentRoomProps.CharacterList;
      SimGameRoom currentRoom = cameraController.CurrentRoomProps;

      SimGameCharacter mimicCharacter = MimicCrewMembers[crewName][currentRoom];

      Main.Logger.Log("[TriggerCustomAnimation] SimGameCharacter mimic is: " + mimicCharacter.name);

      Animator currentAnimator = mimicCharacter.GetComponent<Animator>();
      currentAnimator.applyRootMotion = enableRootMotion;
      currentAnimator.runtimeAnimatorController = runtimeAnimatorController;
    }

    // public void TriggerCustomAnimation(string crewName, string animationName, bool enableRootMotion) {
    //   ReplaceAllVanillaCrewWithMimics(); // test entry point

    //   GameObject crewMemberPrefab = AssetBundleLoader.GetAsset<GameObject>("ec-assets-bundle", $"EC" + crewName);
    //   RuntimeAnimatorController runtimeAnimatorController = AssetBundleLoader.GetAsset<RuntimeAnimatorController>("ec-assets-bundle", $"{animationName}TestAnimationController");

    //   if (runtimeAnimatorController == null) {
    //     Main.Logger.LogError($"[Actions.TriggerCustomAnimation] '{animationName}TestAnimationController' was not found in bundle");
    //     return;
    //   }

    //   if (crewMemberPrefab == null) {
    //     Main.Logger.LogError($"[Actions.TriggerCustomAnimation] 'EC{crewName}' was not found in bundle");
    //     return;
    //   }

    //   SimGameCameraController cameraController = UnityGameInstance.Instance.Game.Simulation.CameraController;
    //   List<SimGameCharacter> characterList = cameraController.CurrentRoomProps.CharacterList;

    //   foreach (SimGameCharacter character in characterList) {
    //     if (character.character.ToString().ToLower().Contains(crewName.ToLower())) {
    //       Animator animator = character.gameObject.GetComponent<Animator>();
    //       if (animator != null) {
    //         GameObject crewMemberGo = GameObject.Instantiate(crewMemberPrefab, character.gameObject.transform.position, character.gameObject.transform.rotation, character.gameObject.transform.parent);
    //         crewMemberGo.transform.localScale = new Vector3(3.4f, 3.4f, 3.4f);

    //         CapsuleCollider originalCapsuleCollider = character.gameObject.GetComponent<CapsuleCollider>();
    //         CapsuleCollider capsuleCollider = crewMemberGo.AddComponent<CapsuleCollider>();
    //         capsuleCollider.center = originalCapsuleCollider.center;
    //         capsuleCollider.radius = originalCapsuleCollider.radius;
    //         capsuleCollider.height = originalCapsuleCollider.height;
    //         capsuleCollider.direction = originalCapsuleCollider.direction;
    //         capsuleCollider.contactOffset = originalCapsuleCollider.contactOffset;

    //         crewMemberGo.name = crewMemberGo.name.Replace("(Clone)", "");
    //         if (character.gameObject.activeSelf) character.gameObject.SetActive(false);

    //         DressCharacter(character.gameObject, crewMemberGo);
    //         Animator currentAnimator = crewMemberGo.GetComponent<Animator>();
    //         currentAnimator.applyRootMotion = enableRootMotion;
    //         currentAnimator.runtimeAnimatorController = runtimeAnimatorController;
    //       }
    //     }
    //   }
    // }

    private static void DressCharacter(GameObject originalCharacter, GameObject mimicCharacter) {
      Transform modelTransform = mimicCharacter.transform.GetChild(0);
      foreach (Transform modelMesh in modelTransform) {
        string goName = modelMesh.gameObject.name;

        SkinnedMeshRenderer mimicSkinnedMeshRenderer = modelMesh.GetComponent<SkinnedMeshRenderer>();
        Main.Logger.Log($"[DressCharacter] Investigating child with goName '{goName}' on go '{originalCharacter.name}'");
        Main.Logger.Log($"[DressCharacter] Found child with goName '{goName}' and name is: " + originalCharacter.transform.Find(goName).gameObject.name);
        SkinnedMeshRenderer originalSkinnedMeshRenderer = originalCharacter.transform.Find(goName).gameObject.GetComponent<SkinnedMeshRenderer>();

        mimicSkinnedMeshRenderer.sharedMaterial = originalSkinnedMeshRenderer.sharedMaterial;
      }
    }
  }
}