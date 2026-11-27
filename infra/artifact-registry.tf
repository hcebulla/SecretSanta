resource "google_artifact_registry_repository" "containers" {
  location      = var.region
  repository_id = "secret-santa"
  description   = "Secret Santa container images"
  format        = "DOCKER"

  docker_config {
    immutable_tags = true
  }

  depends_on = [
    google_project_service.apis
  ]
}