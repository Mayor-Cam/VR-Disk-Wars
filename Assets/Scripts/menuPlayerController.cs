#if ENABLE_UNET
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;

public class menuPlayerController : MonoBehaviour {
	public XRNode xrHead, xrLeftHand, xrRightHand;
	// Use this for initialization
	public Transform camera, leftHand, rightHand;
	public PointerScript pointerScript;
	public Transform pointerSelecting;
	public NetworkManager manager;
	bool searchMatch = false;
	bool requestMatches = false;
	void Start () {
		manager = GameObject.Find("Network Manager").GetComponent<NetworkManager>();
	}
	
	// Update is called once per frame
	void Update () {
		camera.transform.localPosition = InputTracking.GetLocalPosition(xrHead);
		leftHand.localPosition = InputTracking.GetLocalPosition(xrLeftHand);
		leftHand.localRotation = InputTracking.GetLocalRotation(xrLeftHand);
		rightHand.transform.localPosition = InputTracking.GetLocalPosition(xrRightHand);
		rightHand.localRotation = InputTracking.GetLocalRotation(xrRightHand); 
		if(pointerScript.selecting.name == "textBackground") {
			pointerSelecting = pointerScript.selecting.parent;
		} 
		else {
			pointerSelecting = null;
		}
		if(Input.GetKey(KeyCode.Space) && !searchMatch) {
			searchMatch = true;
		}
		if(pointerSelecting != null && !searchMatch) {
			if(Input.GetAxis("TriggerRight") == 1f) {
				switch (pointerSelecting.name) {
					case "FindMatch":
							searchMatch = true;
						break;
					case "Practice":
							//Practice();
						break;
					default:
							Application.Quit();
						break;
				}
			}
		}

		if(searchMatch) FindMatch();
	}
	void FindMatch() {
		if(!NetworkServer.active && !NetworkClient.active) {
			if (manager.matchMaker == null) {
				manager.StartMatchMaker();
			}
			else {
				if(manager.matchInfo == null) {
					if(manager.matches == null && !requestMatches)  {
						requestMatches = true;
						manager.matchMaker.ListMatches(0,20, "VRDiskWars", true, 0, 0, OnMatchList);
					}
				}
			}
		}
	}
	void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
		if(matches.Count > 0) {
			manager.matchName = matches[0].name;
			manager.matchSize = (uint)matches[0].currentSize;
			manager.matchMaker.JoinMatch(matches[0].networkId, "", "", "", 0, 0, manager.OnMatchJoined);
		}
		else {
			manager.matchMaker.CreateMatch("VRDiskWars", 2, true, "", "", "", 0, 0, manager.OnMatchCreate);
		}
	}
}
#endif //ENABLE_UNET