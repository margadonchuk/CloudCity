-- Update VDI images for 20 persons products
-- Note: ProductType.VDI = 8 in the enum

-- Netherlands
UPDATE Products 
SET ImageUrl = '/images/vdi_start20_1.png'
WHERE Slug = 'vdi-start-netherlands-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt20_1.png'
WHERE Slug = 'vdi-standard-netherlands-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro20_1.png'
WHERE Slug = 'vdi-pro-netherlands-20' AND Configuration LIKE '%20 человек%';

-- Germany/France/Poland
UPDATE Products 
SET ImageUrl = '/images/vdi_start20_2.png'
WHERE Slug = 'vdi-start-europe-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt20_2.png'
WHERE Slug = 'vdi-standard-europe-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro20_2.png'
WHERE Slug = 'vdi-pro-europe-20' AND Configuration LIKE '%20 человек%';

-- USA/Canada/Asia
UPDATE Products 
SET ImageUrl = '/images/vdi_start20_3.png'
WHERE Slug = 'vdi-start-usa-asia-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_standrt20_3.png'
WHERE Slug = 'vdi-standard-usa-asia-20' AND Configuration LIKE '%20 человек%';

UPDATE Products 
SET ImageUrl = '/images/vdi_pro20_3.png'
WHERE Slug = 'vdi-pro-usa-asia-20' AND Configuration LIKE '%20 человек%';

