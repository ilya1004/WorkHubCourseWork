import { Project } from "./project.interface";
import { EmployerUser } from "../employer/employer-user.interface";

export interface ProjectReport {
  id: string;
  description: string | null;
  status: ReportStatus;
  projectId: string;
  project: Project | null;
  reporterUserId: string;
  reviewerUserId: string | null;
}

export enum ReportStatus {
  Sent = 0,
  Reviewed = 1,
  Rejected = 2,
}