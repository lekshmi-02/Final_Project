

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class CubeGenerator : MonoBehaviour
{
    public GameObject cubePrefab;
    public GameObject indexPrefab;
    public TMP_InputField userInputField;
    public Button submitButton;
    public float spacing = 1f;
    public Color textColor = Color.white;
    public Color indexColor = Color.black;
    public Color comparisonColor = Color.yellow;
    public Color indcompColor = Color.red;
    public Color sortedColor = Color.green;
    public float sortingDelay = 1f;
    public float swapSpeed = 12f;
    public GameObject iterationCanvasPrefab;
    public GameObject bubbleInputCanvas; // Reference to the BubbleInputCanvas
    public Camera mainCamera;

    private TextMeshProUGUI iterationText;
    private GameObject[] cubes;
    private GameObject[] indexes;
    private bool sortingInProgress = false;
    private bool paused = false;

    private void Start()
    {
        submitButton.onClick.AddListener(GenerateCubes);
    }



    public void GenerateCubes()
    {

        if (sortingInProgress)
        {
            return;
        }

        sortingInProgress = true;

        // Hide the BubbleInputCanvas

        // Destroy previous cubes and indexes
        DestroyCubesAndIndexes();

        string userInput = userInputField.text;
        string[] numbers = userInput.Split(',');
        bubbleInputCanvas.SetActive(false);

        // Calculate total width
        float totalWidth = (numbers.Length - 1) * spacing;
        float startX = -totalWidth / 2f;
        Vector3 iterationTextPosition = new Vector3(startX - 0.5f, 0f, 0f);

        CreateIterationText(iterationTextPosition);

        float currentX = startX;

        cubes = new GameObject[numbers.Length];
        indexes = new GameObject[numbers.Length];

        for (int i = 0; i < numbers.Length; i++)
        {
            Vector3 cubePosition = new Vector3(currentX, 0f, 0f);
            Vector3 indexPosition = new Vector3(currentX, -250f, 0f);

            GameObject cube = Instantiate(cubePrefab, cubePosition, Quaternion.identity);
            GameObject index = Instantiate(indexPrefab, indexPosition, Quaternion.identity);

            currentX += spacing * 12;

            cubes[i] = cube;
            indexes[i] = index;

            // Set up cube and index UI
            SetupCubeAndIndexUI(cube, index, numbers[i], i);
        }
        //PositionCamera();
        // Start sorting coroutine
        StartCoroutine(BubbleSortCoroutine());

        // Focus main camera on the cubes
        //FocusMainCameraOnCubes();
    }

    private void DestroyCubesAndIndexes()
    {
        if (cubes != null)
        {
            foreach (GameObject cube in cubes)
            {
                Destroy(cube);
            }
        }

        if (indexes != null)
        {
            foreach (GameObject index in indexes)
            {
                Destroy(index);
            }
        }



    }

    private void SetupCubeAndIndexUI(GameObject cube, GameObject index, string number, int indexNumber)
    {
        Canvas canvas = cube.GetComponentInChildren<Canvas>();
        Canvas indexCanvas = index.GetComponentInChildren<Canvas>();

        if (canvas != null && indexCanvas != null)
        {
            TextMeshProUGUI textMesh = canvas.GetComponentInChildren<TextMeshProUGUI>();
            TextMeshProUGUI indexTextMesh = indexCanvas.GetComponentInChildren<TextMeshProUGUI>();

            if (textMesh != null && indexTextMesh != null)
            {
                textMesh.text = number;
                textMesh.color = textColor;
                textMesh.alignment = TextAlignmentOptions.Center;

                indexTextMesh.text = indexNumber.ToString();
                indexTextMesh.color = indexColor;
                indexTextMesh.alignment = TextAlignmentOptions.Center;

                float cubeSize = 24.2f;
                float fontSizeMultiplier = 4f;
                textMesh.fontSize = Mathf.RoundToInt(cubeSize * fontSizeMultiplier);
                indexTextMesh.fontSize = Mathf.RoundToInt(cubeSize * 3.5f);
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in the canvas of the cube or index prefab.");
            }
        }
        else
        {
            Debug.LogError("Canvas component not found in the children of the cube or index prefab.");
        }
    }

    private void CreateIterationText(Vector3 position)
    {
        GameObject iterationCanvasGO = Instantiate(iterationCanvasPrefab, Vector3.zero, Quaternion.identity);
        iterationCanvasGO.transform.SetParent(transform, false);

        iterationText = iterationCanvasGO.GetComponentInChildren<TextMeshProUGUI>();

        if (iterationText != null)
        {
            iterationText.text = "Iteration: 1";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in the IterationCanvas prefab.");
        }

        iterationCanvasGO.transform.localPosition = position;
    }

    private IEnumerator BubbleSortCoroutine()
    {
        yield return new WaitForSeconds(sortingDelay);

        int n = cubes.Length;

        for (int iteration = 1; iteration < n; iteration++)
        {
            bool swapped = false;

            if (iterationText != null)
            {
                iterationText.text = "Iteration: " + iteration;
            }

            for (int i = 1; i < n; i++)
            {
                indexes[i].GetComponentInChildren<TextMeshProUGUI>().color = indcompColor;
                indexes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = indcompColor;

                int currentValue = int.Parse(cubes[i].GetComponentInChildren<TextMeshProUGUI>().text);
                int previousValue = int.Parse(cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().text);

                if (currentValue < previousValue)
                {
                    cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;
                    cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = comparisonColor;

                    Vector3 tempPosition = cubes[i].transform.position;
                    Vector3 newPosition = cubes[i - 1].transform.position;
                    newPosition.y += 1f;

                    while (cubes[i].transform.position != newPosition)
                    {
                        cubes[i].transform.position = Vector3.MoveTowards(cubes[i].transform.position, newPosition, Time.deltaTime * swapSpeed);
                        cubes[i - 1].transform.position = Vector3.MoveTowards(cubes[i - 1].transform.position, tempPosition, Time.deltaTime * swapSpeed);
                        yield return null;
                    }

                    GameObject tempCube = cubes[i];
                    cubes[i] = cubes[i - 1];
                    cubes[i - 1] = tempCube;

                    swapped = true;
                    cubes[i].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                    cubes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = textColor;
                }

                indexes[i].GetComponentInChildren<TextMeshProUGUI>().color = indexColor;
                indexes[i - 1].GetComponentInChildren<TextMeshProUGUI>().color = indexColor;

                if (paused)
                {
                    yield return new WaitWhile(() => paused == true);
                }

                yield return new WaitForSeconds(0.5f);
            }

            cubes[n - iteration].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        }

        cubes[0].GetComponentInChildren<TextMeshProUGUI>().color = Color.green;
        sortingInProgress = false;
        iterationText.text = "Sorted";
    }
    /*
    private void PositionCamera()
    {
        // Calculate the center position of the cubes
        float totalWidth = (cubes.Length - 1) * spacing;
        float startX = -totalWidth / 2f;
        float centerY = -1000f;

        // Set camera position to view the cubes
        mainCamera.transform.position = new Vector3(startX, centerY, -10f); // Adjust -10f based on your scene
        mainCamera.transform.rotation = Quaternion.identity;
    }
    */
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
        StopAllCoroutines();
        sortingInProgress = false;
        paused = false;
        GenerateCubes();
    }
}