﻿using LearnLink.Core.Models;

namespace LearnLink.Core.Interfaces
{
	public interface IAttendanceManagementService
    {

        Task<bool> AddAttendanceAsync(AttendanceFormViewModel viewModel, string userId);

        Task<AttendanceFormViewModel> GetAttendanceForEditAsync(int? id);

        Task<bool> UpdateAttendanceAsync(int id, AttendanceFormViewModel viewModel);

        Task<AttendanceViewModel> GetAttendanceForDeleteAsync(int id);

        Task<bool> DeleteAttendanceAsync(int id);
    }
}
