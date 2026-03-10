

namespace Common.Domain
{
    public enum CourseStatus
    {
        Draft = 0,
        Published = 1,
        Unpublished = 2,
    }

    public enum CourseType
    {
        None = 0,
        SimulationCourse = 1,
        CareerVideo = 2
    }

    public enum ProgressStatusEnum
    {
        None = 0,
        Inprogress = 1,
        Done = 2
    }

    public enum ApplyStatus
    {
        Apply = 0,
        ReApply = 1,
    }
}
