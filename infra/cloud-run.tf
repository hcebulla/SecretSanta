
resource "google_cloud_run_v2_service" "web" {
  name                = "secret-santa"
  location            = var.region
  deletion_protection = true
  ingress             = "INGRESS_TRAFFIC_ALL"

  template {
    service_account = google_service_account.runtime.email

    scaling {
      min_instance_count = 0
      max_instance_count = 2
    }

    containers {
      image = var.bootstrap_image

      ports {
        container_port = 8080
      }

      resources {
        limits = {
          cpu    = "1"
          memory = "512Mi"
        }

        cpu_idle = true
      }

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = "Production"
      }
    }
  }

  lifecycle {
    ignore_changes = [
      template[0].containers[0].image,
      traffic
    ]
  }

  depends_on = [
    google_project_service.apis
  ]
}

resource "google_cloud_run_v2_service_iam_member" "public_access" {
  project  = var.project_id
  location = var.region
  name     = google_cloud_run_v2_service.web.name
  role     = "roles/run.invoker"
  member   = "allUsers"
}
