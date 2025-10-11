using UnityEngine;
using System.Collections;

public class StudentManager : MonoBehaviour
{
    private StudentController[] students;
    private StudentController activeStudent;
    private bool isActionRunning = false;

    void Start()
    {
        // find all students automatically
        students = FindObjectsOfType<StudentController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isActionRunning)
        {
            StartCoroutine(RandomStudentStand());
        }
    }

    IEnumerator RandomStudentStand()
    {
        isActionRunning = true;

        // pick random student
        activeStudent = students[Random.Range(0, students.Length)];

        // make him stand
        activeStudent.ToggleSitStand(false);

        // wait 5 seconds
        yield return new WaitForSeconds(5f);

        // make him sit again
        activeStudent.ToggleSitStand(true);

        isActionRunning = false;
    }
}
