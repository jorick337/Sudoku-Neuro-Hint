using Game.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Classes
{
    public class GridBlock : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private CellManager[] cellManagers;
        [SerializeField] private Image image;

        public CellManager[] CellManagers => cellManagers;
        public Image SelectedBlockImage => image;
    }
}
