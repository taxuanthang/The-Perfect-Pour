using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class FluidPercentageDisplay : MonoBehaviour
{
    [Header("References")]
    public GameObject cupObject; // The cup GameObject that contains multiple detectors
    public TextMeshProUGUI displayText;
    public CupFactory cupFactory; // Reference to get spawned cups
    
    [Header("Display Settings")]
    public string prefix = "Container: ";
    public string suffix = "%";
    public string completionString = "STOP";
    public int decimalPlaces = 1;
    public float smoothingSpeed = 5f; // Higher value = faster smoothing
    
    [Header("Multiple Detector Settings")]
    public bool requireAllDetectorsFull = true; // If true, ALL detectors must be full
    public bool showDetectorCount = false; // Show "3 / 5" detectors full
    public bool autoTrackLatestCup = true; // Automatically track the latest spawned cup
    
    [Header("Color Settings")]
    public Color startColor = Color.black;
    public Color endColor = Color.white;
    public Color thresholdColor = Color.red;
    
    [Header("Debug")]
    public bool showDebugLogs = false;
    
    private float currentDisplayValue = 0f;
    private List<FluidDetector> detectors = new List<FluidDetector>();
    private float lastDetectorScanTime = 0f;
    private float detectorScanInterval = 0.5f; // Scan every 0.5 seconds
    
    void Start()
    {
        // Auto-find TextMeshProUGUI if not set
        if (displayText == null)
        {
            displayText = GetComponent<TextMeshProUGUI>();
            if (displayText == null)
            {
                Debug.LogError("No TextMeshProUGUI component found!");
                enabled = false;
                return;
            }
        }
        
        // Try to find CupFactory if not assigned
        if (cupFactory == null)
        {
            cupFactory = FindFirstObjectByType<CupFactory>();
            if (cupFactory != null && showDebugLogs)
                Debug.Log("CupFactory found!");
        }
        
        FindAllDetectors();
    }

    void FindAllDetectors()
    {
        detectors.Clear();
        
        // If auto-tracking, get the latest cup from CupFactory
        if (autoTrackLatestCup && cupFactory != null)
        {
            var cups = cupFactory.GetCups();
            if (cups != null && cups.Count > 0)
            {
                // Get the most recently spawned cup
                var latestCup = cups.OrderByDescending(c => c.spawnTime).FirstOrDefault();
                if (latestCup != null && latestCup.cupObject != null)
                {
                    cupObject = latestCup.cupObject;
                    if (showDebugLogs)
                        Debug.Log($"Tracking latest cup: {cupObject.name}");
                }
            }
        }
        
        if (cupObject != null)
        {
            // Find all FluidDetector components in the cup and its children
            detectors = cupObject.GetComponentsInChildren<FluidDetector>().ToList();
            
            if (showDebugLogs)
                Debug.Log($"Found {detectors.Count} FluidDetector(s) in {cupObject.name}");
        }
        else
        {
            // If still no cup, try to find ANY detectors in the scene as fallback
            var allDetectors = FindObjectsByType<FluidDetector>(FindObjectsSortMode.None);
            if (allDetectors.Length > 0)
            {
                detectors = allDetectors.ToList();
                
                if (showDebugLogs)
                    Debug.Log($"Found {detectors.Count} FluidDetector(s) in scene (fallback)");
            }
        }
    }

    void Update()
    {
        if (displayText == null) return;
        
        // Periodically re-scan for detectors instead of every frame
        if (detectors.Count == 0 && Time.time >= lastDetectorScanTime + detectorScanInterval)
        {
            FindAllDetectors();
            lastDetectorScanTime = Time.time;
            
            if (detectors.Count == 0)
            {
                displayText.text = "Waiting...";
                displayText.color = startColor;
                return;
            }
        }
        
        // Clean up null detectors (in case some were destroyed)
        detectors.RemoveAll(d => d == null);
        
        if (detectors.Count == 0)
        {
            displayText.text = "No detectors";
            displayText.color = startColor;
            return;
        }
        
        // Calculate statistics across all detectors
        int fullCount = 0;
        float totalPercentage = 0f;
        
        foreach (var detector in detectors)
        {
            if (detector.isFluidPresent)
            {
                fullCount++;
            }
            
            // Calculate percentage for this detector
            float percentage = (detector.currentDensity / detector.densityThreshold) * 100f;
            totalPercentage += Mathf.Min(percentage, 100f);
        }
        
        // Average percentage across all detectors
        float averagePercentage = totalPercentage / detectors.Count;
        
        // Smooth the display value
        currentDisplayValue = Mathf.Lerp(currentDisplayValue, averagePercentage, Time.deltaTime * smoothingSpeed);
        
        // Format the text with the specified decimal places
        string percentageText = currentDisplayValue.ToString($"F{decimalPlaces}");
        
        // Determine if cup is "full" based on requirements
        bool isFull = requireAllDetectorsFull 
            ? (fullCount == detectors.Count)  // ALL detectors must be full
            : (fullCount > 0);                 // At least ONE detector is full
        
        // Update text and color
        if (!isFull)
        {
            // Build display text
            string displayString = $"{prefix}{percentageText}{suffix}";
            
            if (showDetectorCount)
            {
                displayString += $" ({fullCount}/{detectors.Count})";
            }
            
            displayText.text = displayString;
            
            // Calculate color based on percentage
            float colorLerpValue = currentDisplayValue / 100f;
            Color currentColor = Color.Lerp(startColor, endColor, colorLerpValue);
            displayText.color = currentColor;
        }
        else
        {
            // All required detectors are full
            displayText.text = completionString;
            displayText.color = thresholdColor;
            
            if (showDebugLogs)
                Debug.Log($"Cup is full! ({fullCount}/{detectors.Count} detectors triggered)");
        }
    }
    
    // Public methods to change cup reference at runtime
    public void SetCupObject(GameObject newCup)
    {
        cupObject = newCup;
        FindAllDetectors();
        currentDisplayValue = 0f; // Reset display value
    }
    
    public int GetDetectorCount()
    {
        return detectors.Count;
    }
    
    public int GetFullDetectorCount()
    {
        return detectors.Count(d => d != null && d.isFluidPresent);
    }
    
    // Force a rescan of detectors
    public void RefreshDetectors()
    {
        FindAllDetectors();
    }
}