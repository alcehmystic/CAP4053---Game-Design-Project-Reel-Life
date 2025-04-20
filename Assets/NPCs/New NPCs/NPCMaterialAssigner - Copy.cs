using UnityEngine;

#if UNITY_EDITOR // Ensure this code only runs in the editor
using UnityEditor; // Needed for EditorUtility
using UnityEditor.SceneManagement; // Potentially needed for marking scene dirty

// This class will hold the data for one material slot assignment
[System.Serializable] // Make this class visible in the Inspector
public class MaterialSlotAssignment
{
    public int slotIndex; // The index of the material slot you want to change
    public Material[] possibleMaterials; // The list of materials that can be assigned to this slot
}

[ExecuteInEditMode] // This makes the script's Awake method run in the editor
public class RandomSlotMaterialAssigner : MonoBehaviour
{
    // Array to hold assignments for different material slots
    public MaterialSlotAssignment[] slotAssignments;

    // This function is called when the script is loaded or an object is enabled in the editor
    void Awake()
    {
        // Check if we are in the editor and not in play mode
        if (Application.isEditor && !Application.isPlaying)
        {
            AssignRandomMaterialsToSlots();
        }
    }

    void AssignRandomMaterialsToSlots()
    {
        // Get the renderer component on this GameObject or in its children
        // Adjust if your renderer is not a child of the root or you have multiple renderers
        Renderer npcRenderer = GetComponentInChildren<Renderer>();

        if (npcRenderer == null)
        {
            Debug.LogWarning("RandomSlotMaterialAssigner: No Renderer found on " + gameObject.name + " or its children.");
            return;
        }

        if (slotAssignments == null || slotAssignments.Length == 0)
        {
            Debug.LogWarning("RandomSlotMaterialAssigner: No Slot Assignments defined on " + gameObject.name);
            return;
        }

        // Get the current materials array from the renderer
        // Use .sharedMaterials to modify the asset directly in the editor
        Material[] currentMaterials = npcRenderer.sharedMaterials;
        bool materialsChanged = false; // Flag to track if any material was actually updated

        foreach (var assignment in slotAssignments)
        {
            // Validate the slot index
            if (assignment.slotIndex < 0 || assignment.slotIndex >= currentMaterials.Length)
            {
                Debug.LogWarning($"RandomSlotMaterialAssigner: Invalid slot index {assignment.slotIndex} specified for " + gameObject.name + ". Renderer has " + currentMaterials.Length + " material slots.");
                continue; // Skip this assignment
            }

            // Validate the list of possible materials for this slot
            if (assignment.possibleMaterials == null || assignment.possibleMaterials.Length == 0)
            {
                Debug.LogWarning($"RandomSlotMaterialAssigner: No possible materials assigned for slot index {assignment.slotIndex} on " + gameObject.name);
                continue; // Skip this assignment
            }

            // Select a random material from the list for this specific slot
            int randomIndex = Random.Range(0, assignment.possibleMaterials.Length);
            Material selectedMaterial = assignment.possibleMaterials[randomIndex];

            // Assign the selected material to the correct slot in the array
            // Only update if the material is different to avoid unnecessary dirtying of the scene
            if (currentMaterials[assignment.slotIndex] != selectedMaterial)
            {
                 currentMaterials[assignment.slotIndex] = selectedMaterial;
                 materialsChanged = true; // Mark that a change occurred
                 Debug.Log($"RandomSlotMaterialAssigner: Assigned material '{selectedMaterial.name}' to slot {assignment.slotIndex} on {gameObject.name}");
            }
            else
            {
                 // Material was already the one selected, no need to change or mark dirty
                 Debug.Log($"RandomSlotMaterialAssigner: Material for slot {assignment.slotIndex} on {gameObject.name} was already '{selectedMaterial.name}'");
            }
        }

        // If any material was changed, re-assign the modified array to the renderer
        // and mark the object/scene as dirty
        if (materialsChanged)
        {
             npcRenderer.sharedMaterials = currentMaterials;

             // Mark the GameObject as dirty so the changes are saved with the scene
             EditorUtility.SetDirty(gameObject);

             // Optional: Mark the scene as dirty as well, though SetDirty on the object
             // is often sufficient when modifying a component on a scene object.
             // EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
    }
}
#endif // End of UNITY_EDITOR block