using System.Collections.Generic;

namespace InterviewSetup.Model;

public record ApiResponse(int Count, List<Wmi> Results);