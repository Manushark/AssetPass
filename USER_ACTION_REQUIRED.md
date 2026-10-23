# AssetPass — User Action Required

This file tracks items that require user intervention to complete.

## Status Legend
- 🔴 **Blocking** — Cannot proceed without this
- 🟡 **Required for Production** — Development can continue with mocks
- 🟢 **Optional Enhancement** — Nice to have, not critical for MVP

---

## 🟡 Email Service Configuration

**Variable:** `EMAIL_PROVIDER_API_KEY`, `EMAIL_FROM_ADDRESS`, `EMAIL_SMTP_HOST`, `EMAIL_SMTP_PORT`  
**Needed for:** Password recovery emails, maintenance reminders, notification delivery  
**Where to get it:** Any email provider (SendGrid, Mailgun, AWS SES, etc.)  
**File:** `.env`  
**Current state:** Mock implementation in development. Emails are logged to console.  
**Protection:** Stored in environment variables, never in source code.

---

## 🟡 Domain & SSL Certificate

**Variable:** `APP_DOMAIN`, `APP_PUBLIC_URL`  
**Needed for:** QR code URLs, public equipment portal, CORS configuration  
**Where to get it:** Domain registrar + SSL provider (or Let's Encrypt)  
**File:** `.env`  
**Current state:** Using `localhost` for development.  
**Protection:** Public information, but production URLs should be configured per environment.

---

## 🟢 WhatsApp Business API

**Variable:** `WHATSAPP_API_KEY`, `WHATSAPP_PHONE_NUMBER_ID`  
**Needed for:** WhatsApp notifications (future feature)  
**Where to get it:** Meta Business Suite → WhatsApp Business API  
**File:** `.env`  
**Current state:** Interface defined, no implementation. Feature flag disabled.  
**Protection:** Stored in environment variables.

---

## 🟢 Cloud File Storage

**Variable:** `STORAGE_PROVIDER`, `STORAGE_CONNECTION_STRING`, `STORAGE_CONTAINER_NAME`  
**Needed for:** Production file storage (photos, documents)  
**Where to get it:** AWS S3, Azure Blob Storage, or similar  
**File:** `.env`  
**Current state:** Using local file system storage for development.  
**Protection:** Stored in environment variables.

---

## 🟢 Payment Processor

**Variable:** `PAYMENT_PROVIDER_API_KEY`, `PAYMENT_WEBHOOK_SECRET`  
**Needed for:** Automated subscription billing  
**Where to get it:** Stripe, PayPal, or local payment processor  
**File:** `.env`  
**Current state:** Subscription management is internal only. No real payment processing.  
**Protection:** Stored in environment variables, webhook signature verification.

---

## 🟡 PostgreSQL Production Credentials

**Variable:** `DATABASE_HOST`, `DATABASE_PORT`, `DATABASE_NAME`, `DATABASE_USER`, `DATABASE_PASSWORD`  
**Needed for:** Production database connection  
**Where to get it:** Hosting provider or self-managed PostgreSQL instance  
**File:** `.env`  
**Current state:** Docker Compose provides a local PostgreSQL instance for development.  
**Protection:** Stored in environment variables, never committed to repository.

---

## 🟡 Production Hosting

**Needed for:** Deploying the application to production  
**Options:** 
- VPS (DigitalOcean, Linode, Hetzner)
- Cloud (AWS, Azure, GCP)
- PaaS (Railway, Render)
**Current state:** Docker Compose ready for local development. Deployment guide prepared.  
**Action:** Choose hosting provider and provide server access.

---

## 🟢 AI/LLM Integration

**Variable:** `AI_PROVIDER_API_KEY`, `AI_MODEL`  
**Needed for:** Voice/text to structured service report (future feature)  
**Where to get it:** OpenAI, Google AI, Anthropic, etc.  
**File:** `.env`  
**Current state:** Interface defined for future implementation. Feature is optional.  
**Protection:** Stored in environment variables. No private data sent without authorization.
