export interface Lifecycle {
  id: string;
  createdAt: string;
  updatedAt: string | null;
  applicationsStartDate: string;
  applicationsDeadline: string;
  workStartDate: string;
  workDeadline: string;
  acceptanceStatus: ProjectAcceptanceStatus;
  projectStatus: ProjectStatus;
  projectId: string;
}

export enum ProjectStatus {
  Published = 0,
  AcceptingApplications = 1,
  WaitingForWorkStart = 2,
  InProgress = 3,
  PendingForReview = 4,
  Completed = 5,
  Expired = 6,
  Cancelled = 7
}

export enum ProjectAcceptanceStatus {
  None = 0,
  Requested = 1,
  Accepted = 2,
}