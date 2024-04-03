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
        const string WIN_MSG = "Party won the battle";
        const string LOSS_MSG = "All party defeated, GAME OVER";

        readonly Action<int> _selectEnemyCallback;
        readonly Action _selectRunActionCallback;
        readonly List<BattleEntity> _enemyBattlers;
        readonly List<BattleEntity> _playerBattlers;
        readonly GameObject _battleMenu;
        readonly GameObject _enemySelectionMenu;
        readonly GameObject _bottomPopUp;
        readonly TextMeshProUGUI _actionTitleText;
        readonly TextMeshProUGUI _bottomPopUpText;
        readonly Button[] _enemySelectionButtons;
        readonly Button _attackButton;
        readonly Button _runButton;
        
        public BattleUI(List<BattleEntity> playerBattlers, List<BattleEntity> enemyBattlers, Action<int> selectEnemyCallback, Action selectRunActionCallback, GameObject battleCanvas)
        {
            _playerBattlers = playerBattlers;
            _enemyBattlers = enemyBattlers;
            _selectEnemyCallback = selectEnemyCallback;
            _selectRunActionCallback = selectRunActionCallback;
            _battleMenu = battleCanvas.transform.GetChild(0).gameObject;
            _attackButton = _battleMenu.transform.GetChild(0).GetComponent<Button>();
            _runButton = _battleMenu.transform.GetChild(1).GetComponent<Button>();
            _actionTitleText = _battleMenu.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            _enemySelectionMenu = battleCanvas.transform.GetChild(1).gameObject;
            _enemySelectionButtons = _enemySelectionMenu.GetComponentsInChildren<Button>();
            _bottomPopUp = battleCanvas.transform.GetChild(2).gameObject;
            _bottomPopUpText = _bottomPopUp.GetComponentInChildren<TextMeshProUGUI>();
        }
        
        public void ShowBattleMenu(int currentPlayer)
        {
            _actionTitleText.text = _playerBattlers[currentPlayer].name + ACTION_MSG;
            _battleMenu.SetActive(true);
            SetAttackButtonAction();
            SetRunButtonAction();
        }
        
        public void ToggleEnemySelectionMenu()
        {
            _enemySelectionMenu.SetActive(!_enemySelectionMenu.activeSelf);
        }

        public void ToggleBottomPoPUp()
        {
            _bottomPopUp.SetActive(!_bottomPopUp.activeSelf);
        }

        public void ShowDamageText(string attackerName, string targetName, int damage)
        {
            _bottomPopUpText.text = $"{attackerName} attacked {targetName} for {damage} damage!";
        }

        public void ShowDefeatedText(string attackerName, string targetName)
        {
            _bottomPopUpText.text = $"{attackerName} defeated {targetName}!";
        }

        public void ShowWinText()
        {
            _bottomPopUpText.text = WIN_MSG;
        }

        public void ShowLossText()
        {
            _bottomPopUpText.text = LOSS_MSG;
        }

        void SetAttackButtonAction()
        {
            _attackButton.onClick.RemoveAllListeners();
            _attackButton.onClick.AddListener(ShowEnemySelectionMenu);
        }

        void SetRunButtonAction()
        {
            _runButton.onClick.RemoveAllListeners();
            _runButton.onClick.AddListener(RunAction);
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
        
        void RunAction() 
        {
            _battleMenu.SetActive(false);
            _selectRunActionCallback.Invoke();
        }
    }
}
