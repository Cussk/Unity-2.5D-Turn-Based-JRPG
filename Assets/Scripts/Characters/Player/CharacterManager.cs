using Characters.Party;
using UnityEngine;

namespace Characters.Player
{
    public class CharacterManager : MonoBehaviour
    {
        const string NPC_JOINABLE_TAG = "NPCJoinable";
        bool _inFrontOfPartyMember;
        GameObject _joinableMember;
        JoinableCharacterScript _joinableCharacterScript;
        PlayerControls _playerControls;
        PartyManager _partyManager;

        public void Init(PlayerControls playerControls, PartyManager partyManager)
        {
            _playerControls = playerControls;
            _partyManager = partyManager;

            _playerControls.Player.Interact.performed += eButton => Interact();
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
            _partyManager.AddMemberToPartyByName(partyMember.memberName);
        }
    }
}
