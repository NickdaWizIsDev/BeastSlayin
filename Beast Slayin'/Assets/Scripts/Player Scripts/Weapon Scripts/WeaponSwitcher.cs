using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[,] weaponPrefabs;  // Array of weapon prefabs

    public GameObject piercerPrefab;
    public GameObject marksmanPrefab;
    public GameObject sharpshooterPrefab;
    public GameObject coreEjectPrefab;
    public GameObject pumpChargePrefab;
    public GameObject magneticPrefab;
    public GameObject overheatPrefab;
    public GameObject elecCannon;

    // Index variables to keep track of the currently selected weapon
    private int currentWeaponIndex = 0;
    private int currentSubWeaponIndex = 0;
    private int currentRevolver;
    private int currentShotgun;
    private int currentNailgun;
    private int currentRailgun;
    private int currentRocketLauncher;

    private GameObject currentWeaponInstance; // Reference to the currently instantiated weapon

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the array of weapon prefabs with the appropriate GameObjects
        weaponPrefabs = new GameObject[5, 3]
        {
            { piercerPrefab, marksmanPrefab, null },
            { coreEjectPrefab, pumpChargePrefab, null },
            { null, null, null },
            { elecCannon, null, null },
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

        currentWeaponIndex = 5;
    }

    void Update()
    {
        if (currentWeaponIndex == 0)
        {
            currentRevolver = currentSubWeaponIndex;
        }            
        if (currentWeaponIndex == 1)
        {
            currentShotgun = currentSubWeaponIndex;
        }
        if (currentWeaponIndex == 2)
        {
            currentNailgun = currentSubWeaponIndex;
        }
        if (currentWeaponIndex == 3)
        {
            currentRailgun = currentSubWeaponIndex;
        }
        if (currentWeaponIndex == 4)
        {
            currentRocketLauncher = currentSubWeaponIndex;
        }

        // Check for input to switch weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectWeapon(0, currentRevolver);  // Switch to the next weapon variant in the first category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon(1, currentShotgun);  // Switch to the next weapon variant in the second category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectWeapon(2, currentNailgun);  // Switch to the next weapon variant in the third category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectWeapon(3, currentRailgun);  // Switch to the next weapon variant in the fourth category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectWeapon(4, currentRocketLauncher);  // Switch to the next weapon variant in the fifth category
        }
        // Add more input checks for other keys and categories as needed

        else if (Input.GetKeyDown(KeyCode.E))
        {
            if(currentWeaponIndex != 5)
                currentSubWeaponIndex = (currentSubWeaponIndex + 1) % weaponPrefabs.GetLength(1);
                SelectWeapon(currentWeaponIndex, currentSubWeaponIndex);
        }        
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
            else
            {
                // If the selected weapon prefab is null, find the first non-null weapon prefab in the category
                for (int i = 0; i < weaponPrefabs.GetLength(1); i++)
                {
                    GameObject weaponPrefab = weaponPrefabs[categoryIndex, i];
                    if (weaponPrefab != null)
                    {
                        selectedWeaponPrefab = weaponPrefab;
                        weaponIndex = i;
                        break;
                    }
                }

                // Instantiate the selected weapon prefab as a new GameObject
                if (selectedWeaponPrefab != null)
                {
                    currentWeaponInstance = Instantiate(selectedWeaponPrefab, transform.position, transform.rotation);
                    currentWeaponInstance.transform.parent = transform; // Set the desired parent object
                    currentWeaponInstance.SetActive(true);
                }
            }

            currentWeaponIndex = categoryIndex;
            currentSubWeaponIndex = weaponIndex;
        }
    }

}
