import { Category } from "./category.interface";
import { Lifecycle } from "./lifecycle.interface";
import { FreelancerApplication } from "./freelancer-application.interface";
import { ProjectReport } from "./project-report";

export interface Project {
  id: string;
  title: string;
  description: string;
  budget: number;
  paymentIntentId: string | null;
  employerUserId: string;
  freelancerUserId: string | null;
  isActive: boolean;
  lifecycle: Lifecycle;
  categoryId: string | null;
  category: Category | null;
  freelancerApplications: FreelancerApplication[];
  reports: ProjectReport[];
}
