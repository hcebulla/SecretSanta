terraform {
  backend "gcs" {
    bucket = "secret-santa-app-2026-tfstate"
    prefix = "terraform/state"
  }
}