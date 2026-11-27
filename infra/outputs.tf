output "artifact_registry_url" {
  description = "Container image registry URL"
  value       = "${var.region}-docker.pkg.dev/${var.project_id}/${google_artifact_registry_repository.containers.repository_id}"
}

output "workload_identity_provider" {
  description = "GitHub Actions identity provider"
  value       = google_iam_workload_identity_pool_provider.github.name
}

output "deployment_service_account" {
  description = "GitHub Actions service account"
  value       = google_service_account.deployment.email
}

output "runtime_service_account" {
  description = "Application runtime service account"
  value       = google_service_account.runtime.email
}