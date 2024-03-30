using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSystem
{
    public class BattleUI
    {
        const string ACTION_MSG = "'s Action:";

        readonly Action<int> _selectEnemyCallback;
        readonly List<BattleEntity> _enemyBattlers;
        readonly GameObject _battleMenu;
        readonly GameObject _enemySelectionMenu;
        readonly TextMeshProUGUI _actionTitleText;
        readonly Button[] _enemySelectionButtons;
        readonly Button _attackButton;
        
        public BattleUI(List<BattleEntity> enemyBattlers, Action<int> selectEnemyCallback, GameObject battleCanvas)
        {
            _enemyBattlers = enemyBattlers;
            _selectEnemyCallback = selectEnemyCallback;
            _battleMenu = battleCanvas.transform.GetChild(1).gameObject;
            _attackButton = _battleMenu.transform.GetChild(0).GetComponent<Button>();
            _actionTitleText = _battleMenu.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            _enemySelectionMenu = battleCanvas.transform.GetChild(2).gameObject;
            _enemySelectionButtons = _enemySelectionMenu.GetComponentsInChildren<Button>();
        }
        
        public void ShowBattleMenu(List<BattleEntity> playerBattlers, int currentPlayer)
        {
            _actionTitleText.text = playerBattlers[currentPlayer].name + ACTION_MSG;
            _battleMenu.SetActive(true);
            SetAttackButtonAction();
        }

        void SetAttackButtonAction()
        {
            _attackButton.onClick.RemoveAllListeners();
            _attackButton.onClick.AddListener(ShowEnemySelectionMenu);
        }

        void ShowEnemySelectionMenu()
        {
            _battleMenu.SetActive(false);
            SetEnemySelectionButtons();
            SetSelectEnemyButtonsAction();
            ToggleEnemySelectionMenu();
        }

        void SetEnemySelectionButtons()
        {
            for (var i = 0; i < _enemySelectionButtons.Length; i++)
            {
                if (i >= _enemyBattlers.Count)
                {
                    _enemySelectionButtons[i].gameObject.SetActive(false);
                }
                else
                {
                    _enemySelectionButtons[i].gameObject.SetActive(true);
                    _enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = _enemyBattlers[i].name;
                }
            }
        }

        void SetSelectEnemyButtonsAction()
        {
            for (var i = 0; i < _enemySelectionButtons.Length; i++)
            {
                var currentEnemyIndex = i;
                var button = _enemySelectionButtons[i];
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => _selectEnemyCallback(currentEnemyIndex));
            }
        }

        public void ToggleEnemySelectionMenu()
        {
            _enemySelectionMenu.SetActive(!_enemySelectionMenu.gameObject.activeSelf);
        }
    }
}
