resource "google_service_account" "deployment" {
  account_id   = "github-deployer"
  display_name = "GitHub Actions Deployment"

  depends_on = [
    google_project_service.apis
  ]
}

resource "google_service_account" "runtime" {
  account_id   = "secret-santa-runtime"
  display_name = "Secret Santa Application Runtime"

  depends_on = [
    google_project_service.apis
  ]
}

resource "google_artifact_registry_repository_iam_member" "publisher" {
  location   = var.region
  repository = google_artifact_registry_repository.containers.repository_id
  role       = "roles/artifactregistry.writer"
  member     = "serviceAccount:${google_service_account.deployment.email}"
}

resource "google_project_iam_member" "cloud_run_developer" {
  project = var.project_id
  role    = "roles/run.developer"
  member  = "serviceAccount:${google_service_account.deployment.email}"
}

resource "google_service_account_iam_member" "runtime_access" {
  service_account_id = google_service_account.runtime.name
  role               = "roles/iam.serviceAccountUser"
  member             = "serviceAccount:${google_service_account.deployment.email}"
}