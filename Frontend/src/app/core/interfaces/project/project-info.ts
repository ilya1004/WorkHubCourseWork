import { ProjectAcceptanceStatus, ProjectStatus } from "./lifecycle.interface";

export interface ProjectInfo {
  id: string;
  title: string;
  description: string;
  budget: number;
  paymentIntentId: string | null;
  employerUserId: string;
  freelancerUserId: string | null;
  categoryId: string | null;
  categoryName: string | null;
  createdAt: string | null;
  updatedAt: string | null;
  applicationsStartDate: string;
  applicationsDeadline: string;
  workStartDate: string;
  workDeadline: string;
  acceptanceStatus: ProjectAcceptanceStatus;
  projectStatus: ProjectStatus;
}