using Characters.Party;
using UI;
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
        
        public void Init(PlayerControls playerControls, PartyManager partyManager, OverWorldPopUpController overWorldPopUpController)
        {
            _playerControls = playerControls;
            _partyManager = partyManager;
            _overWorldPopUpController = overWorldPopUpController;
            
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
            var memberName = partyMember.memberName;
            _partyManager.AddMemberToPartyByName(memberName);
            StartCoroutine(_overWorldPopUpController.DisplayPopUpBanner());
            _overWorldPopUpController.SetPartyJoinedText(memberName);
            _joinableCharacterScript.CheckIfJoined();
        }
    }
}
