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

variable "bootstrap_image" {
  description = "Initial container image used to create Cloud Run"
  type        = string

  default = "us-west1-docker.pkg.dev/secret-santa-app-2026/secret-santa/web@sha256:04e6d96a04d707ea3ac94592a6aefe972ddd1dbc1ba5b852d3a04dae4c37af18"
}
