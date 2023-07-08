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
    private int currentRevolver = 4;
    private int currentShotgun = 4;
    private int currentNailgun = 4;
    private int currentRailgun = 4;
    private int currentRocketLauncher = 4;

    private GameObject currentWeaponInstance; // Reference to the currently instantiated weapon

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the array of weapon prefabs with the appropriate GameObjects
        weaponPrefabs = new GameObject[5, 3]
        {
            { piercerPrefab, marksmanPrefab, null },
            { coreEjectPrefab, pumpChargePrefab, null },
            { magneticPrefab, overheatPrefab, null },
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
    }

    void Update()
    {
        // Check for input to switch weapons
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentRevolver == 4 || currentRevolver >= 2)
                currentRevolver = 0;
            else if (currentRevolver != 3)
                currentRevolver++;
            SelectWeapon(0, currentRevolver);  // Switch to the next weapon variant in the first category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (currentShotgun == 4 || currentShotgun >= 2)
                currentShotgun = 0;
            else if (currentShotgun != 3)
                currentShotgun++;
            SelectWeapon(1, currentShotgun);  // Switch to the next weapon variant in the second category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (currentNailgun == 4 || currentNailgun >= 2)
                currentNailgun = 0;
            else if (currentNailgun != 3)
                currentNailgun++;
            SelectWeapon(2, currentNailgun);  // Switch to the next weapon variant in the third category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currentRailgun == 4 || currentRailgun >= 2)
                currentRailgun = 0;
            else if (currentRailgun != 3)
                currentRailgun++;
            SelectWeapon(3, currentRailgun);  // Switch to the next weapon variant in the fourth category
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (currentRocketLauncher == 4 || currentRocketLauncher >= 2)
                currentRocketLauncher = 0;
            else if (currentRocketLauncher != 3)
                currentRocketLauncher++;
            SelectWeapon(4, currentRocketLauncher);  // Switch to the next weapon variant in the fifth category
        }
        // Add more input checks for other keys and categories as needed
        if (currentWeaponIndex == 0)
            currentSubWeaponIndex = currentRevolver;
        if (currentWeaponIndex == 1)
            currentSubWeaponIndex = currentShotgun;
        if (currentWeaponIndex == 2)
            currentSubWeaponIndex = currentNailgun;
        if (currentWeaponIndex == 3)
            currentSubWeaponIndex = currentRailgun;
        if (currentWeaponIndex == 4)
            currentSubWeaponIndex = currentRocketLauncher;
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
