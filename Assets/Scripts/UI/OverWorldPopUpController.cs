using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OverWorldPopUpController
    {
        const float FADE_SPEED = 2.5f;
        const float TARGET_ALPHA = 0.95f;
        const float START_ALPHA = 0f;

        readonly GameObject _popUpBanner;
        readonly Image _backgroundImage;
        readonly TextMeshProUGUI _bannerText;

        public OverWorldPopUpController(GameObject popUpCaGameObject)
        {
            _popUpBanner = popUpCaGameObject.transform.GetChild(0).gameObject;
            _backgroundImage = _popUpBanner.transform.GetChild(0).GetComponent<Image>();
            _bannerText = _backgroundImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            
            SetColorAlpha(_backgroundImage, START_ALPHA);
            SetColorAlpha(_bannerText, START_ALPHA);
        }

        public void SetPartyJoinedText(string memberName)
        {
            _bannerText.text = $"{memberName} has joined the Party";
        }

        public IEnumerator DisplayPopUpBanner()
        {
            _popUpBanner.SetActive(true);
            
            yield return new WaitForEndOfFrame();
           
            while (_backgroundImage.color.a < TARGET_ALPHA)
            {
                SetColorAlpha(_backgroundImage, Mathf.MoveTowards(_backgroundImage.color.a, TARGET_ALPHA, Time.deltaTime * FADE_SPEED));
                SetColorAlpha(_bannerText, Mathf.MoveTowards(_bannerText.color.a, TARGET_ALPHA, Time.deltaTime * FADE_SPEED));
                yield return null;
            }
    
            yield return new WaitForSeconds(1);
            
            while (_backgroundImage.color.a > START_ALPHA)
            {
                SetColorAlpha(_backgroundImage, Mathf.MoveTowards(_backgroundImage.color.a, START_ALPHA, Time.deltaTime * FADE_SPEED));
                SetColorAlpha(_bannerText, Mathf.MoveTowards(_bannerText.color.a, START_ALPHA, Time.deltaTime * FADE_SPEED));
                yield return null;
            }
            
            SetColorAlpha(_backgroundImage, START_ALPHA);
            SetColorAlpha(_bannerText, START_ALPHA);
            _popUpBanner.SetActive(false);
        }

        static void SetColorAlpha(Graphic graphic, float alpha)
        {
            var color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}
