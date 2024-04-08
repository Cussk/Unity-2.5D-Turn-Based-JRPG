using UnityEngine;

namespace Characters.Party
{
    public class JoinableCharacterScript : MonoBehaviour
    {
        public PartyMemberInfo memberToJoin;
        [SerializeField] GameObject interactPrompt;
        PartyManager _partyManager;

        void Awake()
        {
            _partyManager = FindFirstObjectByType<PartyManager>();
            CheckIfJoined();
        }

        public void ShowInteractPrompt(bool showPrompt)
        {
            interactPrompt.SetActive(showPrompt);
        }

        public void CheckIfJoined()
        {
            var currentParty = _partyManager.GetCurrentPartyMembers();

            foreach (var member in currentParty)
            {
                if (member.memberName != memberToJoin.memberName) continue;
                
                gameObject.SetActive(false);
            }
        }
    }
}
