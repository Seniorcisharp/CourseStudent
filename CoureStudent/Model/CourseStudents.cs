namespace CoureStudent.Model
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
    }

    public class Student
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
    }
}
