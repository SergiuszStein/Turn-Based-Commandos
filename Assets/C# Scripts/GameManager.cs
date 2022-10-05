using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace C__Scripts
{
    public class GameManager : MonoBehaviour
    {
        [Header("All Scenes")]
        public int _playerNumber = 2;

        [Header("Start Scene")]
        [SerializeField] private TextMeshProUGUI _playerCountDisplayText;

        [Header("Match Scene")]
        public string _playerWin;
        
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        //
        //START GAME SCENE FUNCTIONS
        //

        public void Start_Game()
        {
            SceneManager.LoadScene(1);
        }

        public void IncreasePlayerNumber()
        {
            _playerNumber = Mathf.Clamp(_playerNumber + 1, 2, 4);

            Update_PlayerNumberDisplay();
        }
    
        public void DecreasePlayerNumber()
        {
            _playerNumber = Mathf.Clamp(_playerNumber - 1, 2, 4);
        
            Update_PlayerNumberDisplay();
        }

        private void Update_PlayerNumberDisplay()
        {
            _playerCountDisplayText.text = _playerNumber.ToString();
        }
        
        public void Exit()
        {
            Application.Quit();
        }
    }
}
