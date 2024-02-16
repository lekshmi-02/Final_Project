using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BubbleSort : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject indexTextPrefab;
    public TMP_InputField userInputField;
    public Button submitButton;
    public float spacing = 2f;
    public Color textColor = Color.white;
    public Color comparisonColor = Color.yellow; // Color for cubes being compared
    public Color sortedColor = Color.green;
    public float sortingDelay = 1f; // Delay before starting the sorting process
    public float swapSpeed = 12f;
    public GameObject iterationCanvasPrefab;

    private TextMeshProUGUI iterationText;
    private GameObject[] cubes;
    private GameObject[] indexTexts;
    private bool sortingInProgress = false;
    private bool paused = false;
    private int iterations = 0;

    private void Start()
    {
        submitButton.onClick.AddListener(GenerateCubes);
    }

    public void GenerateCubes()
    {
        if (sortingInProgress)
        {
            // If sorting is already in progress, ignore the button click
            return;
        }

        sortingInProgress = true; // Set flag to indicate sorting is in progress

        if (cubes != null)
        {
            // Clean up previously generated cubes
            foreach (GameObject cube in cubes)
            {
                Destroy(cube);
            }
        }

        string userInput = userInputField.text;
        string[] numbers = userInput.Split(',');

        Debug.Log("Number of elements in numbers array: " + numbers.Length); // Debug print

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;

        // Calculate starting position
        float startX = -totalWidth / 2f;
        // Calculate position for iteration text
        Vector3 iterationTextPosition = new Vector3(startX - 0.5f, 0f, 0f); // Adjust position as needed

        // Create and position the iteration text
        CreateIterationText(iterationTextPosition);

        // Initialize currentX to starting position
        float currentX = startX;

        cubes = new GameObject[numbers.Length]; // Initialize the array to store cube references
        indexTexts = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            // Use the current position for each cube
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);
            Vector3 indexPosition = new Vector3(currentX, -4f, 0f);

            Debug.Log("Position of cube " + (i + 1) + ": " + cubePosition); // Debug print

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            GameObject index = Instantiate(indexTextPrefab, indexPosition, Quaternion.identity);

            // Update currentX for the next cube
            currentX += spacing * 22; // Double the spacing to ensure even spacing

            cubes[i] = cube; // Store reference to the cube in the array
            indexTexts[i] = index;

            // Access the TextMeshPro component inside the canvas of the cube prefab and update its text
            Canvas canvas = cube.GetComponentInChildren<Canvas>();
            Canvas indexcanvas = index.GetComponentInChildren < Canvas>();
            if (canvas != null)
            {
                TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI indexTextMesh = indexcanvas.GetComponentInChildren<TextMeshProUGUI>();

                if (textMesh != null)
                {
                    textMesh.text = numbers[i];
                    textMesh.color = textColor;
                    textMesh.alignment = TextAlignmentOptions.Center;

                    // Set font size based on cube size
                    float cubeSize = 24.2f; // Adjust this value based on your cube size
                    float fontSizeMultiplier = 2.5f; // Adjust this multiplier as needed
                    textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                    
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube prefab.");
                }
                if (indexTextMesh != null)
                {
                    indexTextMesh.text = (i + 1).ToString();
                    //indexTextMesh.text.fontSize = Mathf.RoundToInt(24.2f * 2.5f);
                }
                else
                {
                    Debug.LogError("TextMeshProUGUI component not found in the children of the index text prefab.");
                }
            }
            else
            {
                Debug.LogError("Canvas component not found in the children of the cube prefab.");
            }
        }

        StartCoroutine(BubbleSortCoroutine());
    }

    private void CreateIterationText(Vector3 position)
    {
        // Instantiate the IterationCanvas prefab
        GameObject iterationCanvasGO = Instantiate(iterationCanvasPrefab, Vector3.zero, Quaternion.identity);

        // Set the parent of the IterationCanvas to be the current GameObject (this object)
        iterationCanvasGO.transform.SetParent(transform, false);

        // Get the TextMeshProUGUI component from the IterationCanvas
        iterationText = iterationCanvasGO.GetComponentInChildren<TextMeshProUGUI>();
        if (iterationText != null)
        {
            // Set initial text value
            iterationText.text = "Iteration: 1";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the IterationCanvas prefab.");
        }

        // Set the position of the IterationCanvas
        iterationCanvasGO.transform.localPosition = position;
    }
    private IEnumerator BubbleSortCoroutine()
    {
        yield return new WaitForSeconds(sortingDelay);

        int n = cubes.Length;

        // Perform bubble sort iterations
        for (int iteration = 1; iteration < n; iteration++)
        {
            bool swapped = false; // Initialize swapped flag
            if(iterationText != null)
            {
                iterationText.text = "Iteration: " + iteration;
            }

            for (int i = 1; i < n; i++)
            {

                // Change color of cubes being compared

                cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
                cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

                // Compare adjacent cubes and swap if necessary
                int currentValue = int.Parse(cubes[i].GetComponentInChildren<TextMeshProUGUI>().text);
                int previousValue = int.Parse(cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().text);

                if (currentValue < previousValue)
                {
                    // Lift and swap cubes
                    Vector3 tempPosition = cubes[i].transform.position;
                    Vector3 newPosition = cubes[i - 1].transform.position;
                    newPosition.y += 1f; // Lift the cube

                    while (cubes[i].transform.position != newPosition)
                    {
                        cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, newPosition, Time.deltaTime * swapSpeed);
                        cubes[i - 1].transform.position = Vector3.MoveTowards(cubes[i - 1].transform.position, tempPosition, Time.deltaTime * swapSpeed);
                        yield return null;
                    }

                    // Swap cube references
                    GameObject tempCube = cubes[i];
                    cubes[i] = cubes[i - 1];
                    cubes[i - 1] = tempCube;

                    swapped = true; // Set swapped flag to true
                }

                // Reset color after comparison
                cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;

                if (paused)
                {
                    yield return new WaitWhile(() => paused == true); // Pause the sorting process
                }

                yield return new WaitForSeconds(0.5f); // Adjust the delay as needed for visualization
            }

            // Increment iterations counter
            iterations++;

            // Check if no swaps occurred in this iteration
            /*if (!swapped)
            {
                break; // Exit the sorting loop if no swaps occurred
            }*/
        }

        sortingInProgress = false;
        iterationText.text = "Sorted";// Sorting is complete
    }

    public void PauseSorting()
    {
        paused = true;
    }

    public void ResumeSorting()
    {
        paused = false;
    }

    public void ReplaySorting()
    {
        StopAllCoroutines(); // Stop any ongoing sorting coroutine
        sortingInProgress = false;
        paused = false;
        iterations = 0; // Reset iterations counter
        GenerateCubes(); // Regenerate cubes and start sorting again
    }
}

     
