using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[,] weaponPrefabs;  // Array of weapon prefabs

    public GameObject piercerPrefab;
    public GameObject marksmanPrefab;
    public GameObject sharpshooterPrefab;

    // Index variables to keep track of the currently selected weapon
    private int currentWeaponIndex = 0;
    private int currentSubWeaponIndex = 0;

    private GameObject currentWeaponInstance; // Reference to the currently instantiated weapon

    // Start is called before the first frame update
    void Start()
    {
        currentSubWeaponIndex = -1;

        // Initialize the array of weapon prefabs with the appropriate GameObjects
        weaponPrefabs = new GameObject[5, 3] {
        { piercerPrefab, marksmanPrefab, sharpshooterPrefab },
        { null, null, null },
        { null, null, null },
        { null, null, null },
        { null, null, null }
    };

        // Print the list of weapon prefabs
        for (int i = 0; i < weaponPrefabs.GetLength(0); i++)
        {
            for (int j = 0; j < weaponPrefabs.GetLength(1); j++)
            {
                GameObject weaponPrefab = weaponPrefabs[i, j];
                if (weaponPrefab != null)
                {
                    Debug.Log($"Category {i}, Weapon {j}: {weaponPrefab.name}");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check for input to switch weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
            SelectWeapon(0, currentSubWeaponIndex);  // Switch to the next weapon variant in the first category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
            SelectWeapon(1, currentSubWeaponIndex);  // Switch to the next weapon variant in the second category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
            SelectWeapon(2, currentSubWeaponIndex);  // Switch to the next weapon variant in the third category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
            SelectWeapon(3, currentSubWeaponIndex);  // Switch to the next weapon variant in the fourth category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
            SelectWeapon(4, currentSubWeaponIndex);  // Switch to the next weapon variant in the fifth category
        }
        // Add more input checks for other keys and categories as needed
    }


    private void SelectWeapon(int categoryIndex, int weaponIndex)
    {
        // Check if the provided indices are valid
        if (categoryIndex >= 0 && categoryIndex < weaponPrefabs.GetLength(0) &&
            weaponIndex >= 0 && weaponIndex < weaponPrefabs.GetLength(1))
        {
            // Destroy the previously instantiated weapon if it exists
            if (currentWeaponInstance != null)
            {
                Destroy(currentWeaponInstance);
            }

            // Get the weapon prefab at the specified indices
            GameObject selectedWeaponPrefab = weaponPrefabs[categoryIndex, weaponIndex];

            // Instantiate the selected weapon prefab as a new GameObject
            if (selectedWeaponPrefab != null)
            {
                currentWeaponInstance = Instantiate(selectedWeaponPrefab, transform.position, transform.rotation);
                currentWeaponInstance.transform.parent = transform; // Set the desired parent object
                currentWeaponInstance.SetActive(true);
            }

            currentWeaponIndex = categoryIndex;
            currentSubWeaponIndex = weaponIndex;
        }
    }
}
