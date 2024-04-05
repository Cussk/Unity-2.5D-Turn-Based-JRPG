using UnityEngine;

namespace Characters.Party
{
    public class JoinableCharacterScript : MonoBehaviour
    {
        public PartyMemberInfo memberToJoin;
        [SerializeField] GameObject interactPrompt;

        public void ShowInteractPrompt(bool showPrompt)
        {
            interactPrompt.SetActive(showPrompt);
        }

        public void CheckIfJoined()
        {
            
        }
    }
}
