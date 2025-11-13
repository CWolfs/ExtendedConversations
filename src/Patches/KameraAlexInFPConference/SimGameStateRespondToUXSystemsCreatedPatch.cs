using UnityEngine;

using Harmony;

using BattleTech;
using BattleTech.UI;

using System.Collections;
using ExtendedConversations.Core;

namespace ExtendedConversations {
  [HarmonyPatch(typeof(SimGameState), "RespondToUXSystemsCreated")]
  public class SimGameStateAttachUXPatch {
    // static void Prefix(SimGameState __instance) {
    //   Actions.MovedKameraInLeopardCommandCenter = false;
    // }

    static void Postfix(SimGameState __instance) {
      GameObject flashpointConferenceRoomGO = GameObject.Find("FlashpointConference");
      bool charactersExist = flashpointConferenceRoomGO.transform.Find("Kamea") != null;

      if (!charactersExist) {
        GameObject leopardConferenceRoomGO = GameObject.Find("LeopardConference");

        GameObject originalKamea = leopardConferenceRoomGO.transform.Find("Kamea").gameObject;
        GameObject originalAlexander = leopardConferenceRoomGO.transform.Find("Alexander").gameObject;

        GameObject copiedKamea = GameObject.Instantiate(originalKamea, flashpointConferenceRoomGO.transform);
        copiedKamea.name = "Kamea";
        copiedKamea.transform.position = new Vector3(copiedKamea.transform.position.x, -20.39f, copiedKamea.transform.position.z);
        foreach (Transform child in copiedKamea.transform) {
          child.gameObject.SetActive(true);
        }

        GameObject copiedAlexander = GameObject.Instantiate(originalAlexander, flashpointConferenceRoomGO.transform);
        copiedAlexander.name = "Alexander";
        copiedAlexander.transform.position = new Vector3(0.35f, -20.317f, copiedAlexander.transform.position.z);
        foreach (Transform child in copiedAlexander.transform) {
          child.gameObject.SetActive(true);
        }

        // Copy lights
        Transform flashpointLightsTransform = flashpointConferenceRoomGO.transform.Find("Lights");
        GameObject originalLight8 = leopardConferenceRoomGO.transform.Find("Lights/Light (8)").gameObject;
        GameObject originalLight9 = leopardConferenceRoomGO.transform.Find("Lights/Light (9)").gameObject;

        GameObject copiedLight8 = GameObject.Instantiate(originalLight8, flashpointLightsTransform);
        copiedLight8.name = "Light (8) Copied";
        copiedLight8.transform.localPosition = new Vector3(-2.016f, 0.9379997f, 3.953f);
        copiedLight8.SetActive(false);

        GameObject copiedLight9 = GameObject.Instantiate(originalLight9, flashpointLightsTransform);
        copiedLight9.name = "Light (9) Copied";
        copiedLight9.transform.localPosition = new Vector3(0.26f, 0.9379997f, 7.018f);
        copiedLight8.SetActive(false);

        UnityGameInstance.Instance.StartCoroutine(DisableCharactersByDefault(copiedKamea, copiedAlexander));
      }
    }

    static IEnumerator DisableCharactersByDefault(GameObject copiedKamea, GameObject copiedAlexander) {
      yield return new WaitForEndOfFrame();
      yield return new WaitForEndOfFrame();

      foreach (Transform child in copiedKamea.transform) {
        child.gameObject.SetActive(false);
      }

      foreach (Transform child in copiedAlexander.transform) {
        child.gameObject.SetActive(false);
      }
    }
  }
}