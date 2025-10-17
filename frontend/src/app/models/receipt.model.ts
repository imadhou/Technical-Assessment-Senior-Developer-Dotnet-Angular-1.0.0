export interface Receipt {
  fileName?: string | null;
  fileUrl?: string | null;
  fileSize: number;
  contentType?: string | null;
  uploadedAt: string; // ISO date string
}
