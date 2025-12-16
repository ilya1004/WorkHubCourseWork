export interface FreelancerProfile {
  firstName: string;
  lastName: string;
  about: string | null;
  stripeAccountId: string | null;
  userId: string;
}