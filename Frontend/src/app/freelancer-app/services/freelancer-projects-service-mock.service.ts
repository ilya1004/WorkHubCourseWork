import { Injectable } from '@angular/core';
import { Project } from "../../core/interfaces/project/project.interface";
import { ProjectStatus } from "../../core/interfaces/project/lifecycle.interface";
import { delay, Observable, of } from "rxjs";
import { PaginatedResult } from "../../core/interfaces/common/paginated-result.interface";

@Injectable({
  providedIn: 'root'
})
export class FreelancerProjectsServiceMockService {

  private mockProjects: Project[] = [
    {
      id: 'proj_001',
      title: 'Разработка интернет-магазина на Angular + .NET',
      description: 'Нужен full-stack разработчик...',
      budget: 250000,
      categoryId: 'cat_3',
      category: { id: 'cat_3', name: 'Веб-разработка' },
      employerId: 'emp_101',
      freelancerId: 'usr_7a9f3e2c',
      freelancerApplications: [],
      lifecycle: {
        id: 'lc_001',
        createdAt: '2025-10-10T14:30:00Z',
        updatedAt: '2025-11-28T09:12:00Z',
        applicationsStartDate: '2025-10-10T00:00:00Z',
        applicationsDeadline: '2025-10-20T23:59:59Z',
        workStartDate: '2025-10-25T00:00:00Z',
        workDeadline: '2025-12-15T23:59:59Z',
        acceptanceRequested: true,
        acceptanceConfirmed: false,
        status: ProjectStatus.InProgress
      }
    },
    {
      id: 'proj_002',
      title: 'Дизайн мобильного приложения (Figma)',
      budget: 80000,
      category: {id: 'cat_5', name: 'UI/UX Дизайн'},
      categoryId: 'cat_5',
      employerId: 'emp_205',
      freelancerId: 'usr_7a9f3e2c',
      freelancerApplications: [],
      lifecycle: {
        id: 'lc_002',
        createdAt: '2025-09-05T10:00:00Z',
        updatedAt: '2025-10-18T16:45:00Z',
        applicationsDeadline: '2025-09-15T23:59:59Z',
        workDeadline: '2025-10-25T23:59:59Z',
        acceptanceRequested: false,
        acceptanceConfirmed: true,
        status: ProjectStatus.Completed,
        applicationsStartDate: "",
        workStartDate: ""
      },
      description: ""
    },
    {
      id: 'proj_003',
      title: 'Интеграция платёжного шлюза Stripe',
      budget: 120000,
      category: {id: 'cat_3', name: 'Веб-разработка'},
      categoryId: 'cat_3',
      employerId: 'emp_108',
      freelancerId: 'usr_7a9f3e2c',
      freelancerApplications: [],
      lifecycle: {
        id: 'lc_003',
        createdAt: '2025-11-15T08:20:00Z',
        updatedAt: '2025-11-30T11:11:00Z',
        workDeadline: '2025-12-20T23:59:59Z',
        acceptanceRequested: false,
        acceptanceConfirmed: false,
        status: ProjectStatus.WaitingForWorkStart,
        applicationsStartDate: "",
        applicationsDeadline: "",
        workStartDate: ""
      },
      description: ""
    },
    {
      id: 'proj_004',
      title: 'Написание технического задания и архитектуры микросервисов',
      budget: 150000,
      category: {id: 'cat_7', name: 'Консультации и архитектура'},
      categoryId: 'cat_7',
      employerId: 'emp_301',
      freelancerId: 'usr_7a9f3e2c',
      freelancerApplications: [],
      lifecycle: {
        id: 'lc_004',
        createdAt: '2025-08-20T12:00:00Z',
        updatedAt: '2025-09-10T14:30:00Z',
        status: ProjectStatus.Completed,
        applicationsStartDate: "",
        applicationsDeadline: "",
        workStartDate: "",
        workDeadline: "",
        acceptanceRequested: false,
        acceptanceConfirmed: false
      },
      description: ""
    }
  ];

  getMyFreelancerProjects(filter: any): Observable<PaginatedResult<Project>> {
    let filtered = [...this.mockProjects];

    if (filter.projectStatus !== null && filter.projectStatus !== '') {
      const statusNum = Number(filter.projectStatus);
      filtered = filtered.filter(p => p.lifecycle.status === statusNum);
    }

    if (filter.employerId) {
      filtered = filtered.filter(p => p.employerId.includes(filter.toLowerCase()));
    }

    const start = (filter.pageNo - 1) * filter.pageSize;
    const end = start + filter.pageSize;
    const items = filtered.slice(start, end);

    const result: PaginatedResult<Project> = {
      items,
      totalCount: filtered.length,
      pageNo: filter.pageNo,
      pageSize: filter.pageSize
    };

    // Имитация сетевой задержки
    return of(result).pipe(delay(500));
  }
}
