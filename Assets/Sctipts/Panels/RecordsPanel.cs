using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Game.Managers;
using Help.UI;
using Help.Classes;
using Game.Classes;

namespace Game.Panels
{
    public class RecordsPanel : MonoBehaviour
    {
        #region CORE

        [Header("Core")]
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject recordRowPrefab;
        [SerializeField] private int maxVisibleRows;

        private DetailedRecord[] _allRecords;

        [Header("Managers")]
        [SerializeField] private ColorThemeManager colorThemeManager;

        private UserManager _userManager;

        #endregion

        #region MONO

        private void Start()
        {
            DisplaySortedRecordsByScore(true);
            colorThemeManager.UpdateUIElementsAndColorTheme();
        }

        private void OnEnable()
        {
            InitializeManagers();
            InitializeValues();
        }

        #endregion

        #region INITIALIZATION

        private void InitializeManagers()
        {
            _userManager = UserManager.Instance;
        }

        private void InitializeValues()
        {
            _allRecords = _userManager.Users.GetAllDetailedRecords();
        }

        #endregion

        #region CORE LOGIC

        private void DisplaySortedRecordsByScore(bool isDescending)
        {
            DetailedRecord[] sortedRecords = _allRecords.GetSortedRecords( detailedRecord => detailedRecord.Record.Score, isDescending, maxVisibleRows);

            UpdateRows(sortedRecords);
        }

        private void UpdateRows(DetailedRecord[] recordsToDisplay)
        {
            ClearExistingRows();

            foreach (var detailedRecord in recordsToDisplay)
            {
                GameObject row = Instantiate(recordRowPrefab, content.transform);
                PopulateRow(row, detailedRecord);
                detailedRecord.RecordRow = row;
            }

            ApplyRowOrder(recordsToDisplay);
        }

        private void ClearExistingRows()
        {
            foreach (Transform child in content.transform) 
                if (child.GetSiblingIndex() > 1) // Не уничтожать первые 2 столбца с названиями
                    Destroy(child.gameObject);
        }
        
        private void ApplyRowOrder(DetailedRecord[] detailedRecords)
        {
            for (int i = 0; i < detailedRecords.Length; i++)
            {
                GameObject recordRow = detailedRecords[i].RecordRow;
                recordRow.transform.SetSiblingIndex(i + 1); // 1 появилась из-за первой колонки в которой строковые имена каждой колонки
            }
        }

        #endregion

        #region UI UPDATES

        private void PopulateRow(GameObject row, DetailedRecord detailedRecord)
        {
            Text[] texts = row.GetComponentsInChildren<Text>().ToArray();
            string[] recordValues = detailedRecord.GetStringValues();

            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].SetText(recordValues[i]);
            }
        }

        #endregion
    }
}