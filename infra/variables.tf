variable "project_id" {
  description = "Google Cloud project ID"
  type        = string
  default     = "secret-santa-app-2026"
}

variable "region" {
  description = "Google Cloud region"
  type        = string
  default     = "us-west1"
}

variable "github_repository_id" {
  description = "GitHub repository numeric ID"
  type        = string
  default     = "1474778367"
}

variable "github_owner_id" {
  description = "GitHub repository owner numeric ID"
  type        = string
  default     = "23067133"
}