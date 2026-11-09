# Product image pattern & image search

## What it does
1. **AddProduct** (`/product/add`): after a gallery image is uploaded, the *image_search* button sends it to SiloAI
   (`POST /api/image-analysis`). The answer is normalized into a JSON *pattern*
   (`summary, category, colors, materials, shapes, keywords`) shown in an editable field and saved in
   `tbl_Products.fld_ProductPattern`.
2. **Image search** (`/product/imagesearch`, sidebar link created by the migration): the user uploads 1–8 images
   (PNG/JPEG/WebP, ≤ 10 MB), the images are analysed with the same prompt and active products that have a pattern are
   ranked by weighted-cosine token similarity (keywords/colors/materials/shapes/category weight 3, summary weight 1).

## Backend
| Piece | Location |
|---|---|
| Pattern parse/serialize/score | `Silo.Application/Features/Product/Services/ProductPatternHelper.cs` |
| Commands/queries | `AnalyzeProductImageCommand`, `SearchProductsByImageQuery` |
| Handlers | `Silo.Application.Api/Features/Product/*` |
| AI client | `IAiApiClient.AnalyzeImagesAsync` → `AiApiHttpClient` |
| Endpoints | `RfidCore/v2/Product/AnalyzeProductImage`, `.../SearchProductsByImage` (Bearer) |
| Save/Get | `WmsBusiness.SaveProduct`, `SSaveProductBatch`, `GetAllProduct` (`ProductPattern`) |

A null/empty `ProductPattern` in `SaveProductCommand` keeps the stored value.

## Database migration
Run `Silo.Domains/Scripts/Migration_ProductImageSearch.sql` (idempotent) on existing databases; it adds
`fld_ProductPattern` and the menu link next to `product/add`. New databases get the column from
`Silo.Api.Db/Tables/tbl_Products.sql`. Texts: `Silo.Domains/Scripts/Seed_tbl_TextResources.sql`.
Non-admin roles receive the page through the Access page (tbl_UserClaim) like any other menu link.

## Tests
`Silo.Tests` (xunit): pattern parsing/similarity/ranking and `AnalyzeProductImageHandler` with a fake AI client.
