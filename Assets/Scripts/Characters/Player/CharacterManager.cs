using System;
using System.Collections.Generic;
using Characters.Party;
using UI;
using Unity.Mathematics;
using UnityEngine;

namespace Characters.Player
{
    public class CharacterManager : MonoBehaviour
    {
        const string NPC_JOINABLE_TAG = "NPCJoinable";
        
        bool _inFrontOfPartyMember;
        PlayerControls _playerControls;
        PartyManager _partyManager;
        OverWorldPopUpController _overWorldPopUpController;
        GameObject _joinableMember;
        JoinableCharacterScript _joinableCharacterScript;
        List<GameObject> _overworldCharacters = new();
        
        public void Init(PlayerControls playerControls, PartyManager partyManager, OverWorldPopUpController overWorldPopUpController)
        {
            _playerControls = playerControls;
            _partyManager = partyManager;
            _overWorldPopUpController = overWorldPopUpController;
            
            _playerControls.Player.Interact.performed += eButton => Interact();
        }

        void Start()
        {
            SpawnOverworldMembers();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag(NPC_JOINABLE_TAG)) return;
            
            _inFrontOfPartyMember = true;
            _joinableMember = other.gameObject;
            _joinableCharacterScript = _joinableMember.GetComponent<JoinableCharacterScript>();
            _joinableCharacterScript.ShowInteractPrompt(true);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag(NPC_JOINABLE_TAG)) return;
            
            _inFrontOfPartyMember = false;
            _joinableCharacterScript.ShowInteractPrompt(false);
            _joinableMember = null;
            _joinableCharacterScript = null;
        }

        void Interact()
        {
            if (!_inFrontOfPartyMember || _joinableMember == null) return;
            
            PartyMemberJoined(_joinableCharacterScript.memberToJoin);
            _inFrontOfPartyMember = false;
            _joinableMember = null;
        }

        void PartyMemberJoined(PartyMemberInfo partyMember)
        {
            var memberName = partyMember.memberName;
            _partyManager.AddMemberToPartyByName(memberName);
            StartCoroutine(_overWorldPopUpController.DisplayPopUpBanner());
            _overWorldPopUpController.SetPartyJoinedText(memberName);
            _joinableCharacterScript.CheckIfJoined();
            SpawnOverworldMembers();
        }

        void SpawnOverworldMembers()
        {
            ClearOverworldVisuals();
            SpawnPartyVisuals();
        }

        void ClearOverworldVisuals()
        {
            foreach (var character in _overworldCharacters)
            {
                Destroy(character);
            }

            _overworldCharacters.Clear();
        }
        
        void SpawnPartyVisuals()
        {
            var currentParty = _partyManager.GetCurrentPartyMembers();
            for (var i = 0; i < currentParty.Count; i++)
            {
                var currentMember = currentParty[i];
                if (i == 0)
                {
                    SpawnPlayerVisuals(currentMember);
                }
                else
                {
                    SpawnFollowerVisuals(currentMember, i);
                }
            }
        }

        void SpawnPlayerVisuals(PartyMember partyMember)
        {
            var player = gameObject;
            var playerVisuals = Instantiate(partyMember.memberOverworldVisualPrefab,
                player.transform.position, quaternion.identity);
                    
            playerVisuals.transform.SetParent(player.transform);
            playerVisuals.GetComponent<MemberFollowAI>().enabled = false;
            player.GetComponent<PlayerController>().SetOverWorldVisuals(playerVisuals.GetComponent<Animator>(), playerVisuals.GetComponent<SpriteRenderer>());
            _overworldCharacters.Add(playerVisuals);
        }
        
        void SpawnFollowerVisuals(PartyMember currentMember, int index)
        {
            var followerPosition = transform.position;
            followerPosition.x -= index;
            var followerVisuals = Instantiate(currentMember.memberOverworldVisualPrefab, followerPosition,
                quaternion.identity);
            followerVisuals.GetComponent<MemberFollowAI>().Init(gameObject.transform, index);
            _overworldCharacters.Add(followerVisuals);
        }
    }
}
