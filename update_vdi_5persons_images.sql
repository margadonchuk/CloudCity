-- Update VDI images for 5 persons products
-- Note: ProductType.VDI = 8 in the enum

-- Netherlands
UPDATE Products 
SET ImageUrl = '/images/vdi_start5_1.png'
WHERE Slug = 'vdi-start-netherlands-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt5_1.png'
WHERE Slug = 'vdi-standard-netherlands-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro5_1.png'
WHERE Slug = 'vdi-pro-netherlands-5' AND Configuration LIKE '%5 человек%';

-- Germany/France/Poland
UPDATE Products 
SET ImageUrl = '/images/vdi_start5_2.png'
WHERE Slug = 'vdi-start-europe-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt5_2.png'
WHERE Slug = 'vdi-standard-europe-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro5_2.png'
WHERE Slug = 'vdi-pro-europe-5' AND Configuration LIKE '%5 человек%';

-- USA/Canada/Asia
UPDATE Products 
SET ImageUrl = '/images/vdi_start5_3.png'
WHERE Slug = 'vdi-start-usa-asia-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt5_3.png'
WHERE Slug = 'vdi-standard-usa-asia-5' AND Configuration LIKE '%5 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro5_3.png'
WHERE Slug = 'vdi-pro-usa-asia-5' AND Configuration LIKE '%5 человек%';

