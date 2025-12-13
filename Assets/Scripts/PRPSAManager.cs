using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PRPSAManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject prpsaOptionsRoot;          // Parent containing Toggles
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI qNumberText;
    public TextMeshProUGUI resultText;           // Optional: show score & category

    // ===================== PRPSA QUESTIONS (CODE ONLY) =====================

    // Not serialized → Inspector CANNOT override
    private readonly string[] questions =
    {
        "While preparing for giving a speech, I feel tense and nervous.",
        "I feel tense when I see the words “speech” and “public speech” on a course outline when studying.",
        "My thoughts become confused and jumbled when I am giving a speech.",
        "Right after giving a speech I feel that I have had a pleasant experience.",
        "I get anxious when I think about a speech coming up.",
        "I have no fear of giving a speech.",
        "Although I am nervous just before starting a speech, I soon settle down after starting and feel calm and comfortable.",
        "I look forward to giving a speech.",
        "When the instructor announces a speaking assignment in class, I can feel myself getting tense.",
        "My hands tremble when I am giving a speech.",
        "I feel relaxed while giving a speech.",
        "I enjoy preparing for a speech.",
        "I am in constant fear of forgetting what I prepared to say.",
        "I get anxious if someone asks me something about my topic that I don’t know.",
        "I face the prospect of giving a speech with confidence.",
        "I feel that I am in complete possession of myself while giving a speech.",
        "My mind is clear when giving a speech.",
        "I do not dread giving a speech.",
        "I perspire just before starting a speech.",
        "My heart beats very fast just as I start a speech.",
        "I experience considerable anxiety while sitting in the room just before my speech starts.",
        "Certain parts of my body feel very tense and rigid while giving a speech.",
        "Realizing that only a little time remains in a speech makes me very tense and anxious.",
        "While giving a speech, I know I can control my feelings of tension and stress.",
        "I breathe faster just before starting a speech.",
        "I feel comfortable and relaxed in the hour or so just before giving a speech.",
        "I do poorer on speeches because I am anxious.",
        "I feel anxious when the teacher announces the date of a speaking assignment.",
        "When I make a mistake while giving a speech, I find it hard to concentrate on the parts that follow.",
        "During an important speech I experience a feeling of helplessness building up inside me.",
        "I have trouble falling asleep the night before a speech.",
        "My heart beats very fast while I present a speech.",
        "I feel anxious while waiting to give my speech.",
        "While giving a speech, I get so nervous I forget facts I really know."
    };

    // Derived, always correct
    private int totalQuestions => questions.Length;



    // Runtime
    private ToggleGroup toggleGroup;
    private Toggle[] toggles;
    private int[] answers;   // stores 1..5
    private int currentIndex = 0;

    // PRPSA scoring sets (1-based indexing)
    private readonly int[] step1Items =
    {
        1,2,3,5,9,10,13,14,19,20,21,22,23,25,
        27,28,29,30,31,32,33,34
    };

    private readonly int[] step2Items =
    {
        4,6,7,8,11,12,15,16,17,18,24,26
    };

    void Awake()
    {
        if (prpsaOptionsRoot == null)
        {
            Debug.LogError("PRPSA Options Root not assigned!");
            return;
        }

        toggleGroup = prpsaOptionsRoot.GetComponent<ToggleGroup>();
        if (toggleGroup == null)
            toggleGroup = prpsaOptionsRoot.AddComponent<ToggleGroup>();

        toggleGroup.allowSwitchOff = true;

        toggles = prpsaOptionsRoot.GetComponentsInChildren<Toggle>(true);
        if (toggles.Length != 5)
            Debug.LogWarning("PRPSA expects exactly 5 radio options.");

        for (int i = 0; i < toggles.Length; i++)
        {
            int captured = i;
            toggles[i].group = toggleGroup;
            toggles[i].onValueChanged.AddListener(isOn =>
            {
                if (isOn) SaveAnswer(captured);
            });
        }

        if (questions == null || questions.Length != totalQuestions)
        {
            Debug.LogWarning("Questions array missing or incorrect length.");
        }

        answers = Enumerable.Repeat(-1, totalQuestions).ToArray();
    }

    void Start()
    {
        // Visual proof that Start() is running in BUILD
        if (questionText != null)
            questionText.text = "DEBUG: Start() called";

        if (qNumberText != null)
            qNumberText.text = "DEBUG";

        // Delay real setup slightly (build-safe)
        Invoke(nameof(SafeInit), 0.2f);
    }

    void SafeInit()
    {
        if (questions == null)
        {
            questionText.text = "DEBUG: questions = NULL";
            return;
        }

        if (questions.Length == 0)
        {
            questionText.text = "DEBUG: questions EMPTY";
            return;
        }

        questionText.text = questions[0];
        qNumberText.text = $"1 of {totalQuestions}";
    }


    void SaveAnswer(int toggleIndex)
    {
        // Convert toggle index (0..4) to Likert score (1..5)
        answers[currentIndex] = toggleIndex + 1;
    }

    void ClearToggles()
    {
        toggleGroup.allowSwitchOff = true;
        foreach (var t in toggles)
            t.isOn = false;
    }

    void ApplySavedAnswer()
    {
        ClearToggles();

        int saved = answers[currentIndex];
        if (saved >= 1 && saved <= 5)
            toggles[saved - 1].isOn = true;
    }

    public void SetQuestion(int index)
    {
        if (index < 0 || index >= totalQuestions) return;

        currentIndex = index;
        questionText.text = questions[currentIndex];
        qNumberText.text = $"{currentIndex + 1} of {totalQuestions}";
        ApplySavedAnswer();
    }

    public void NextQuestion()
    {
        if (currentIndex < totalQuestions - 1)
            SetQuestion(currentIndex + 1);
    }

    public void PreviousQuestion()
    {
        if (currentIndex > 0)
            SetQuestion(currentIndex - 1);
    }

    // ===================== PRPSA SCORING =====================

    public int CalculatePRPSAScore()
    {
        int step1Total = 0;
        int step2Total = 0;

        foreach (int q in step1Items)
        {
            int val = answers[q - 1];
            if (val < 1) return -1; // unanswered
            step1Total += val;
        }

        foreach (int q in step2Items)
        {
            int val = answers[q - 1];
            if (val < 1) return -1;
            step2Total += val;
        }

        int score = 72 - step2Total + step1Total;

        if (score < 34 || score > 170)
        {
            Debug.LogError("PRPSA score out of range. Check responses.");
        }

        return score;
    }

    public string GetAnxietyCategory(int score)
    {
        if (score > 131) return "High Public Speaking Anxiety";
        if (score < 98)  return "Low Public Speaking Anxiety";
        return "Moderate Public Speaking Anxiety";
    }

    // Call this from SUBMIT button
    public void SubmitPRPSA()
    {
        int score = CalculatePRPSAScore();
        if (score == -1)
        {
            Debug.LogWarning("Please answer all questions before submitting.");
            return;
        }

        string category = GetAnxietyCategory(score);

        Debug.Log($"PRPSA Score: {score} | Category: {category}");

        if (resultText != null)
        {
            resultText.text =
                $"PRPSA Score: {score}\n" +
                $"Category: {category}";
        }
    }
}
